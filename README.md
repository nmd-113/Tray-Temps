# TrayTemps

[![GitHub license](https://img.shields.io/github/license/YourUsername/TrayTemps?style=flat-square)](LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/YourUsername/TrayTemps?style=flat-square)](https://github.com/YourUsername/TrayTemps/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/YourUsername/TrayTemps?style=flat-square)](https://github.com/YourUsername/TrayTemps/network/members)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/YourUsername/TrayTemps?style=flat-square)](https://github.com/YourUsername/TrayTemps/releases/latest)

A lightweight and customizable Windows utility that displays your **CPU and GPU temperatures** directly in your system's **notification area (system tray)**. Keep an eye on your hardware's health at a glance without cluttering your desktop!

---

## ✨ Features

* **Real-time Temperature Monitoring:** Get instant temperature readings for your CPU and GPU.
* **Customizable Tray Icons:**
    * Display CPU and GPU temperatures in separate, dedicated tray icons.
    * Choose from a variety of **colors** for each temperature display.
    * Adjust **font size and family** to suit your preference.
* **Configurable Update Interval:** Set how frequently the temperatures refresh.
* **Minimalist Design:** Stays out of your way, providing crucial info without distracting pop-ups.
* **Persistent Settings:** Your preferences are saved and loaded automatically.
* **Optional Autostart & Silent Mode:** Seamlessly integrate TrayTemps into your Windows startup.
* **Easy Installation & Uninstallation:** Simple setup and a straightforward removal process, including full cleanup.
* **Lightweight:** Built with efficiency in mind, using [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for reliable hardware data.

---

## 📸 Screenshots

*(Replace these with your actual screenshots. You can drag and drop images directly into your GitHub repository and then link them here, or use image hosting services.)*

**Main Window:**
![TrayTemps Main Window](https://raw.githubusercontent.com/YourUsername/TrayTemps/main/screenshots/main_window.png)

**CPU & GPU in Tray:**
![TrayTemps CPU GPU Tray](https://raw.githubusercontent.com/YourUsername/TrayTemps/main/screenshots/tray_icons.png)

---

## 🚀 Getting Started

### Prerequisites

* Windows 10 or newer.
* .NET Framework 4.7.2 or later (usually pre-installed or prompted if missing).

### Installation

The easiest way to get TrayTemps running is to download the latest release:

1.  Go to the [Releases page](https://github.com/YourUsername/TrayTemps/releases).
2.  Download the `TrayTemps.zip` file from the latest release.
3.  Extract the contents of the zip file to a folder of your choice (e.g., `C:\Program Files\TrayTemps`).
4.  Run `TrayTemps.exe`.

**For Autostart (Recommended):**
Once the application is running, go to the **Settings** tab within the TrayTemps window and check the "**Autostart with Windows**" option. This will install TrayTemps to a standard program location (`C:\Program Files\TrayTemps`), set it to launch silently on logon, and create a desktop shortcut for convenience. The app will restart automatically after this process.

### Uninstallation

To completely remove TrayTemps from your system:

1.  Open the TrayTemps application.
2.  Go to the **Settings** tab.
3.  Uncheck the "**Autostart with Windows**" option.
4.  You will be prompted with removal options:
    * **Yes:** Removes all installed files, shortcuts, and startup entries.
    * **No:** Only removes the startup entry, leaving the installed files.
    * **Cancel:** Does nothing.
    Choose **Yes** for a complete cleanup.

---

## ⚙️ Usage & Configuration

Once launched, TrayTemps will appear in your system tray. You can:

* **Double-click** any of the TrayTemps icons (CPU, GPU, or the main TrayTemps icon) to bring up the main window.
* **Right-click** the main TrayTemps icon for quick options like `Exit` or showing the main window.

In the main window, you can adjust settings:

* **CPU Tray / GPU Tray Enable:** Toggle visibility of individual CPU/GPU temperature icons in the tray.
* **CPU Tray Color / GPU Tray Color:** Select the text color for the respective tray icons.
* **Font Size Tray / Font Family Tray:** Customize the appearance of the numbers on the tray icons.
* **Update Interval:** Choose how often the temperatures update (in seconds).
* **Autostart with Windows:** Enable or disable automatic launch with Windows (includes installation/uninstallation).

---

## 🤝 Contributing

Contributions are welcome! If you have suggestions, bug reports, or want to contribute code, please feel free to:

1.  **Open an issue** for bugs or feature requests.
2.  **Fork the repository** and create a pull request.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgements

* [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for providing a robust library for hardware monitoring.
