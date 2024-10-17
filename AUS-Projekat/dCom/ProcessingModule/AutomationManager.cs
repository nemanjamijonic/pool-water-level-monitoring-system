﻿using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorker;
        private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
			this.delayBetweenCommands = configuration.DelayBetweenCommands;
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorker = new Thread(AutomationWorker_DoWork);
			automationWorker.Name = "Aumation Thread";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorker.Start();
		}


		private void AutomationWorker_DoWork()
		{
			EGUConverter egu = new EGUConverter();
			PointIdentifier nivoVode = new PointIdentifier(PointType.ANALOG_OUTPUT, 1000);
			PointIdentifier pumpa1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2005);
			PointIdentifier pumpa2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2006);
			PointIdentifier ventil = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2002);
			PointIdentifier stop = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2000);
			List<PointIdentifier> list = new List<PointIdentifier> { nivoVode, pumpa1, pumpa2 ,ventil, stop };
			while (!disposedValue)
			{
				List<IPoint> points = storage.GetPoints(list);


				// prva pumpa radi druga ne
				if (points[1].RawValue == 1) 
				{
					int value = (int)egu.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation,points[0].RawValue);
					value += 160;
					if (value < points[0].ConfigItem.HighLimit)
					{
						processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, nivoVode.Address, value);
					}
					else 
					{
                        processingManager.ExecuteWriteCommand(points[1].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, pumpa1.Address, 0);
                    }

				}

				// druga pumpa radi prva ne
				if (points[2].RawValue == 1) 
				{
                    int value = (int)egu.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation, points[0].RawValue);
                    value += 80;
                    if (value < points[0].ConfigItem.HighLimit)
                    {
                        processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, nivoVode.Address, value);
                    }
                    else
                    {
                        processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, pumpa2.Address, 0);
                    }

                }

				if (points[3].RawValue == 1 && points[0].RawValue >6000)
                {
                    int value = (int)egu.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation, points[0].RawValue);
                    value -= 50;
                    if (value > points[0].ConfigItem.LowLimit)
                    {
                        processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, nivoVode.Address, value);
                    }
                    else
                    {
                        processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, ventil.Address, 0);
                    }

                }

                for (int i = 0; i < delayBetweenCommands; i += 10) 
				{
					automationTrigger.WaitOne();
				}

            }
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
