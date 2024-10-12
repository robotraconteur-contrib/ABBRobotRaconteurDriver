<p align="center"><img src="https://raw.githubusercontent.com/robotraconteur/robotraconteur/refs/heads/master/docs/figures/logo-header.svg"></p>

# Robot Raconteur ABB IRC5 Driver

## Introduction

Robot Raconteur standard robot driver for ABB robots using the IRC5 controller.

This driver communicates with the robot using the Externally Guided Motion (EGM). EGM provides real-time
streaming control of the robot position. This driver uses the real-time control to directly control the robot
motion. There is another driver [abb_motion_program_exec](https://github.com/rpirobotics/abb_motion_program_exec)
that uses motion program commands instead of direct position control. The `abb_motion_program_exec` driver uses
the built-in motion program interpreter in the IRC5 controller to execute motion programs instead of directly
controlling the robot position. Use whichever driver is most appropriate for your application.

Example driver clients are in the `examples/` directory. This driver supports jog, position, and trajectory
command modes for the standard `com.robotraconteur.robotics.robot.Robot` service type.

The [Robot Raconteur Training Simulator](https://github.com/robotraconteur-contrib/robotraconteur_training_sim) contains a simulated ABB IRB 1200 robot in the multi-robot scene.

## Connection Info

The default connection information is as follows. These details may be changed using `--robotraconteur-*` command
line options when starting the service. Also see the
[Robot Raconteur Service Browser](https://github.com/robotraconteur/RobotRaconteur_ServiceBrowser) to detect
services on the network.

- URL: `rr+tcp://localhost:58651?service=robot`
- Device Name: `abb_robot`
- Node Name: `abb_robot`
- Service Name: `robot`
- Root Object Type:
  - `com.robotraconteur.robotics.robot.Robot`

## Command Line Arguments

The following command line arguments are available:

* `--robot-info-file=` - The robot info file. Info files are available in the `config/` directory. See [robot info file documentation](https://github.com/robotraconteur/robotraconteur_standard_robdef/blob/master/docs/info_files/robot.md)
* `--robot-name=` - Overrides the robot device name. Defaults to `abb_robot`.

The [common Robot Raconteur node options](https://github.com/robotraconteur/robotraconteur/wiki/Command-Line-Options) are also available.

## Running the driver

Zip files containing the driver are available on the
[Releases](https://github.com/robotraconteur-contrib/ABBRobotRaconteurDriver/releases) page.
Download the zip file and extract it to a directory.
The .NET 6.0 runtime is required to run the driver. This driver will run on Windows and Linux.

The driver can be run using the following command:

```
ABBRobotRaconteurDriver.exe --robot-info-file=config/abb_1200_5_90_robot_default_config.yml
```

Use the `dotnet` command to run the driver on Linux:

```
dotnet ABBRobotRaconteurDriver.dll --robot-info-file=config/abb_1200_5_90_robot_default_config.yml
```

Use the appropriate robot info file for your robot.

## Running the driver using docker

On Linux it is possible to run the driver using docker. The following command will run the driver using the
`abb_1200_5_90_robot_default_config.yml` robot info file:

```
sudo docker run --rm --net=host --privileged -v /var/run/robotraconteur:/var/run/robotraconteur -v /var/lib/robotraconteur:/var/lib/robotraconteur wasontech/abb-robotraconteur-driver /opt/abb_robotraconteur_driver/ABBRobotRaconteurDriver --robot-info-file=/config/abb_1200_5_90_robot_default_config.yml
```

 It may be necessary to mount a docker "volume" to access configuration yml files that are not included in the docker image.
 See the docker documentation for instructions on mounting a local directory as a volume so it can be accessed inside the docker.

## Robot Setup Instructions

**This driver requires the IRC5 EGM control option.**

These instructions assume that the robot is fully operational, and has the EGM option installed. Before starting, make sure that standard RAPID programs are running normally. Consult the ABB documentation and examples for instructions on how to test the robot.

*Note: If you have Robot Studio installed and connected to the robot, it is easier to follow instructions 4 - 6 below instead of the teach pendant based instructions in this section.*

1. Configure robot settings. Complete the following:
   1. Configure the control computer IP address
      1. Select (menu) -> Control Panel
      2. Select "Configuration"
      3. Select "Topics" -> "Communication"
      4. Select "IP Setting"
      5. Select "Add"
      6. Set "Interface" to WAN
      7. Set IP address of IRC5 Controller
      8. Tap "OK"
   2. Configure the control computer IP address
      1. Select (menu) -> Control Panel
      2. Select "Configuration"
      3. Select "Topics" -> "Communication"
      4. Select "Transmission Protocol"
      5. Select "UCdevice"
      6. Set "Remote Device" to the IP address of the computer running the ABB Robot Raconteur Driver
      7. Tap "OK"
   3. Configure the EGM settings
      1. Select (menu) -> Control Panel
      2. Select "Configuration"
      3. Select "Topics" -> "Motion"
      4. Select "External Motion Interface"
      5. Tap "Add"
      6. Set "Name" to "conf1"
      7. Set "Level" to "Raw"
      8. Set "Do Not Restart after Motors Off" to "Yes"
      9. Set "Default Ramp Time" to 0.1
      10. Tap "Ok"
2. Load the RAPID program. Complete the following:
   1. Select (menu) -> Production Window
   2. Select "Load Module"
   3. Load (.pgf) program
      1. (Note: If you have Robot Studio it is much easier to transfer the program using the RAPID editor on a PC. See Step 5 in the Robot Studio Setup Instructions)
      2. (Note: .pgf could be created from RobotStudio, save to .pgf on controller)
3. Run the RAPID program
   1. Select (menu) -> Production Window
   2. Tap "PP to Main"
   3. If the robot is in manual mode, grasp the enabling switch
   4. Push the "Motors On" button on the controller
   5. Press "Play" on the teach pendant (physical button)
   6. Start the ABB Robot Raconteur Driver program

To start the program in the future or recover from an error, repeat Step 3. The ABB Robot Raconteur Driver program may be started before or after the robot, and should reconnect after an error.

## Robot Studio Setup Instructions

These instructions use the ABB IRB 1200-7/0.7 robot as an example. Other robots can be substituted for this model when configuring the simulator.

1. Select File ribbon tab -> New -> Solution With Sation and Virtual Controller
   1. Set "Solution Name" and "Location" to desired save path
   2. Select "Create New" with the desired Robot Model. For this example, select "IRB 1200 7kg 0.7m"
   3. Check "Customize Options"
   4. Click "Create"
2. "Change Options" will now display. Complete the following:
   1. Type "EGM" into the filter
   2. Select "Engineering Tools" under "System Options"
   3. Check "689-1 Externally Guided Motion (EGM)"
   4. Click "Ok"
3. Select "IRB1200_7_70_STD_02" when prompted to "Select library ..."
4. Configure robot settings. Complete the following:
   1. Select Controller ribbon tab -> Configuration -> Communication
      1. Select "Transmission Protocol"
      2. Double click on "UCdevice"
      3. Set the "Remote Address" to the IP address of the control computer running the Robot Raconteur ABB Driver
      4. Click "Ok"
   2. Select Controller ribbon tab -> Configuration -> Motion
      1. Select "External Motion Interface Data"
      2. Right click in the window and select "New External Motion Interface Data..."
      3. Set "Name" to "conf1"
      4. Set "Do Not Restart after Motors Off" to "Yes"
      5. Set "Level" to "Raw"
      6. Set "Default Ramp Time" to 0.1
5. Configure the RAPID program. Complete the following
   1. Select the "RAPID" ribbon tab
   2. On the Controller tab on the left, expand "RAPID" -> "T_ROB1", and double click "Module 1"
   3. Cut and paste the contents rapid/egm.mod into T_ROB1/Module1, completely replacing the contents of Module1
   4. Click "Apply" on the ribbon
6. Run the program
   1. Select Controller ribbon tab -> Operating Mode
   2. Select "Auto" in the "Operating Mode" window
   3. Select the "RAPID" ribbon tab
   4. Click "Program Pointer" -> "Set Program Pointer to Main in all tasks"
   5. Select "Run Mode" -> "Continuous"
   6. Click "Start"
   7. Start the ABB Robot Raconteur Driver

At this point, the robot and driver should be running. Save the workspace. In starting the simulation in the future, or when recovering from an error, repeat Step 6.

## Acknowledgment

This work was supported in part by the Advanced Robotics for Manufacturing ("ARM") Institute under Agreement Number W911NF-17-3-0004 sponsored by the Office of the Secretary of Defense. The views and conclusions contained in this document are those of the authors and should not be interpreted as representing the official policies, either expressed or implied, of either ARM or the Office of the Secretary of Defense of the U.S. Government. The U.S. Government is authorized to reproduce and distribute reprints for Government purposes, notwithstanding any copyright notation herein.

This work was supported in part by the New York State Empire State Development Division of Science, Technology and Innovation (NYSTAR) under contract C160142.

![](https://github.com/robotraconteur/robotraconteur/blob/master/docs/figures/arm_logo.jpg?raw=true)
![](https://github.com/robotraconteur/robotraconteur/blob/master/docs/figures/nys_logo.jpg?raw=true)
