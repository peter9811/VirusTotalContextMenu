# VirusTotal Context Menu - Right click on a file to scan it

## Features

* Based on VirusTotalNet (<https://github.com/Genbox/VirusTotalNet>) to get reports and scan files.
* Provides detailed feedback after a scan, including the number of positive detections and the total number of scans.
* Includes a progress bar or spinner to indicate that a scan is in progress.
* Adds a notification system to inform users when the scan is complete.
* Implements specific error messages to help users understand what went wrong during the scan.
* Adds retry logic for network-related errors to improve reliability.
* Logs errors to a file for easier troubleshooting and support.
* Allows users to configure the API key directly from the application.
* Supports scanning multiple files at once.
* Provides an option to schedule regular scans for specific files or directories.

## How to use

1. Download the latest release from <https://github.com/Genbox/VirusTotalContextMenu/releases>
2. Unpack the ZIP file to where you want the application. It is portable.
3. Get an API key from VirusTotal.com and put it inside `appsettings.json` or configure it directly from the application.
4. Register the context menu handler by right-clicking the `VirusTotalContextMenu.exe` file and selecting "Run as Administrator".
5. Now you can right-click any file and select "VT Scan" to scan it using VirusTotal.

## Notes

* You can use `VirusTotalContextMenu.exe --register` and `--unregister` command line arguments as well.
* VirusTotal limits the number of requests to 4 per minute.
* VirusTotal also limits the file size to 32 MB.
* It sends your file to VirusTotal if they don't already have it.
* You can schedule regular scans for specific files or directories using the scheduling feature.
