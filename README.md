# Pool Water Level Monitoring System

## Project Description

This project simulates the control of a water tank system with configurable parameters. The system controls the inflow and outflow of water in the tank and handles safety mechanisms such as overflow protection and low water level alarms.

The tank has the following properties:

- **Length**: 2 meters
- **Width**: 1.5 meters
- **Height**: 4 meters
- **Maximum volume**: 12,000 liters
- **Initial water level**: 6,500 liters

### System Operations

- **Water Inflow**: Controlled by digital switches (P1 and P2), which can adjust the inflow speed to 0, 80, 160, or 240 liters per second depending on the switch states.
- **Water Outflow**: When the water level exceeds 2 meters, the drainage system can remove water at 50 liters per second if the valve (V1) is open.
- **Safety Features**:
  - If the water level exceeds 3.5 meters, a **HighAlarm** is triggered, and automatic drainage is initiated.
  - If the water level falls below 1 meter, a **LowAlarm** is triggered.
  - An **AbnormalValue** alarm is triggered if the system is not in its nominal state.

### Control Mechanism

The system allows for manual control of the pump (inflow) and the valve (outflow). However, it ensures that the water tank is either being filled or drained, but not both simultaneously. A STOP switch controls whether the pump or the valve is active:

- If the STOP switch is set to 1, the pump is turned off, and the valve is enabled.
- If the STOP switch is set to 0, the valve is closed, and the pump can be activated.

### Configuration

The system uses the following configuration parameters:

- **RTU Slave Address**: 15
- **TCP Protocol Port**: 25252

The system reads and writes to digital outputs (coils) and analog outputs (holding registers) to manage the water level and control operations. It supports alarms and automatic drainage when certain thresholds are exceeded.

### Key Features

- Simulates water inflow and outflow based on configurable parameters.
- Ensures the system remains in a safe state with automatic alarms and actions.
- Supports manual and automated control of the pump and drainage system.

## Participants

- **Nemanja Mijonić PR138/2020** - [nemanjamijonic]
- **Srdjan Bogićević PR139/2020** - [blackhood10]
