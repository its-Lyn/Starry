# The setup script, can be done without.

import os
import shutil
import subprocess
import platform
import sys


help_str: str = """
Starry Setup Script
====================

Description:
    This script is used to install and uninstall Starry.

Usage:
    sudo python3 setup.py [install|uninstall|help]

    install - Compiles and moves Starry to your local binary directory.
    uninstall - Removes Starry from your local binary directory.
    help - Displays this help message.
"""


def print_err():
    print("\033[91mERR\033[0m")


def print_ok():
    print("\033[92mOK\033[0m")


def ask_perm(action: str):
    if action.lower() != "y" and action.lower() != "yes":
        exit(1)

    if os.getuid() != 0:
        print("You need to run this script as sudo.")
        exit(1)


def main():
    accepted_args: list[str] = ["install", "uninstall", "help"]
    if len(sys.argv) <= 1:
        print("\033[91mERROR:\033[0m Argument not provided. Please specify an argument: [install, uninstall, help]");
        exit(1)
    
    if sys.argv[1] not in accepted_args:
        print("\033[91mERROR:\033[0m Argument doesnt exist. Please specify an existing argument: [install, uninstall, help]");
        exit(1)

    match sys.argv[1]:
        case "install":
            ask_perm(input("Starry will be compiled and moved to /usr/local/bin, for this you need to run as sudo. Do you wish to continue? [Y/n] "))

            print("Compiling Starry... ", end="", flush=True)

            if shutil.which("dotnet") is None:
                print_err()

                print("dotnet is not installed. Please install it.")
                exit(1)

            try:
                subprocess.run(["dotnet", "publish", "-c", "Release", "-p:PublishSingleFile=true"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
                print_ok()
            except Exception as e:
                print_err()

                print(f"Compilation failed with the following error: {e}")
                exit(1)

            print("Moving Starry to /usr/local/bin... ", end="", flush=True)
            try:
                match platform.machine():
                    case "x86_64":
                        shutil.move("Starry/bin/Release/net8.0/linux-x64/publish/Starry", "/usr/local/bin/starry")
                    case "aarch64":
                        shutil.move("Starry/bin/Release/net8.0/linux-arm64/publish/Starry", "/usr/local/bin/starry")
            except Exception as e:
                print_err()

                print(f"Moving failed with the following error: {e}")
                exit(1)

            print_ok()
        case "uninstall":
            ask_perm(input("Starry will be deleted from /usr/local/bin, for this you need to run as sudo. Do you wish to continue? [Y/n] "))

            print("Removing Starry from /usr/local/bin... ", end="", flush=True)

            try:
                os.remove("/usr/local/bin/starry")
                print_ok()
            except Exception as e:
                print_err()

                print(f"Removing failed with the following error: {e}")
                exit(1)
        case "help":
            print(help_str)


# Actual beginning of the setup.
if __name__ == "__main__":
    main()