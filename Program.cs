// Copyright 2020 Rensselaer Polytechnic Institute
//                Wason Technology, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Mono.Options;
using RobotRaconteurWeb;
using RobotRaconteurWeb.InfoParser;

namespace ABBRobotRaconteurDriver
{
    class Program
    {
        static int Main(string[] args)
        {

            bool shouldShowHelp = false;
            string robot_info_file = null;

            var options = new OptionSet {
                { "robot-info-file=", n => robot_info_file = n },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null }
            };

            List<string> extra;
            try
            {
                // parse the command line
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write("ABBRobotRaconteurDriver: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `ABBRobotRaconteurDriver --help' for more information.");
                return 1;
            }

            if (shouldShowHelp)
            {
                Console.WriteLine("Usage: ABBRobotRaconteurDriver [Options+]");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            if (robot_info_file == null)
            {
                Console.WriteLine("error: robot-info-file must be specified");
                return 1;
            }


            var robot_info = RobotInfoParser.LoadRobotInfoYamlWithIdentifierLocks(robot_info_file);
            using (robot_info.Item2)
            {

                

                using (var robot = new ABBRobot(robot_info.Item1))
                {
                    robot._start_robot();
                    using (var node_setup = new ServerNodeSetup("ABB_robot", 58651))
                    {


                        RobotRaconteurNode.s.RegisterService("abb_robot", "com.robotraconteur.robotics.robot", robot);

                        Console.WriteLine("Press enter to exit");
                        Console.ReadKey();

                        RobotRaconteurNode.s.Shutdown();
                    }
                }
            }

            return 0;

        }
    }
}
