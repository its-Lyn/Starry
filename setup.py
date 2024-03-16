# The setup script, can be done without.

import os
import shutil
import subprocess
import platform
import sys
from pathlib import Path


help_str: str = """
Starry Setup Script
====================

Description:
    This script is used to install and uninstall Starry.

Usage:
    (sudo) python setup.py [build|install|uninstall|rconf|help]
    
    build     - Compiles Starry.
    install   - Moves Starry to your local binary directory.
    uninstall - Removes Starry from your local binary directory.
    rconf     - Revomes Starry's configuration folder.
    help      - Displays this help message.
"""


def print_err():
    print("\033[91mERR\033[0m")


def print_ok():
    print("\033[92mOK\033[0m")


def ask_perm(action: str):
    if action.lower() != "y" and action.lower() != "yes":
        exit(1)
    
    try:
        if os.getuid() != 0:
            print("You need to run this script as sudo.")
            exit(1)
    except AttributeError:
        import ctypes
        if ctypes.windll.shell32.IsUserAnAdmin() == 0:
            print("You need to run this script as admin.")
            exit(1)


def main():
    accepted_args: list[str] = ["build", "install", "uninstall", "help", "rconf"]
    if len(sys.argv) <= 1:
        print("\033[91mERROR:\033[0m Argument not provided. Please specify an argument: [build, install, uninstall, rconf, help]");
        exit(1)
    
    if sys.argv[1] not in accepted_args:
        print("\033[91mERROR:\033[0m Argument doesnt exist. Please specify an existing argument: [build, install, uninstall, rconf, help]");
        exit(1)

    match sys.argv[1]:
        case "build":
            action: str =input("Starry will be compiled. Do you wish to continue? [Y/n] ")
            if action.lower() != "y" and action.lower() != "yes":
                exit(1)

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
        case "install":
            print("Moving Starry to ", end="", flush=True)
            try:
                system: str = platform.system();
                match system:
                    case "Windows":
                        path: str = "C:/Windows/system32"
                        print(f"{path}... ", end="", flush=True)   
                        match platform.machine():
                            case "AMD64":
                                shutil.move("Starry/bin/Release/net8.0/win-x64/publish/Starry.exe", f"{path}/starry.exe")
                            case _:
                                print_err()
                                print(f"Unrecognised CPU arch: {platform.machine()}")
                                
                                exit(1)
                    case "Linux":
                        print("/usr/local/bin... ", end="", flush=True)
                        match platform.machine():
                            case "x86_64":
                                shutil.move("Starry/bin/Release/net8.0/linux-x64/publish/Starry", "/usr/local/bin/starry")
                            case "aarch64":
                                shutil.move("Starry/bin/Release/net8.0/linux-arm64/publish/Starry", "/usr/local/bin/starry")
                            case _:
                                print_err()
                                print(f"Unrecognised CPU arch: {platform.machine()}")
                                
                                exit(1)
                    case _:
                        print_err()
                        print("Unrecognised operating system.")

                        exit(1)
            except Exception as e:
                print_err()
                print(f"Moving failed with the following error: {e}")

                exit(1)

            print_ok()
        case "uninstall":
            ask_perm(input("Starry will be deleted from binary path, as well as it's config, for this you need to run as sudo. Do you wish to continue? [Y/n] "))

            print("Removing Starry from binary path... ", end="", flush=True)

            try:
                if platform.system() == "Windows":
                    os.remove("C:/Windows/system32/starry.exe")

                    print_ok()

                    return

                os.remove("/usr/local/bin/starry")

                print_ok()
            except Exception as e:
                print_err()

                print(f"Removing failed with the following error: {e}")
                exit(1)
        case "rconf":
            print("Removing Starry config... ", end="", flush=True)
            try:
                star_path: str = "";

                xdg_conf: str = os.getenv("XDG_CONFIG_HOME", "NOT_SET")
                if xdg_conf != "NOT_SET":
                    star_path = os.path.join(xdg_conf, "Starry")
                else:
                    home_path: str = Path.home();
                    star_path = os.path.join(home_path, ".config", "Starry")

                shutil.rmtree(star_path)

                print_ok()
            except Exception as e:
                print_err()

                print(f"Removing failed with the following error: {e}")
                exit(1)
        case "help":
            print(help_str)


if __name__ == "__main__":
    main()