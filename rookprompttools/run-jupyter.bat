@echo off

REM Change to directory of current script.
cd /D "%~dp0"

call setup-environment.bat

jupyter lab
