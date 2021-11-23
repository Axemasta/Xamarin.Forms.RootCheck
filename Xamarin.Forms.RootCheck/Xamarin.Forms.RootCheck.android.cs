﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Java.IO;
using Java.Lang;
using RootCheck.Core;
using System.Collections.Generic;

namespace Xamarin.Forms.RootCheck
{
    /// <summary>
    /// Android implementation of IChecker
    /// </summary>
    public class RootChecker : IChecker
    {
        static readonly string BINARY_SU = "su";
        static readonly string BINARY_BUSYBOX = "busybox";
        static readonly string BINARY_MAGISK = "magisk";

        static readonly List<string> KnownRootAppsPackages = new List<string> {
            "com.noshufou.android.su",
            "com.noshufou.android.su.elite",
            "eu.chainfire.supersu",
            "com.koushikdutta.superuser",
            "com.thirdparty.superuser",
            "com.yellowes.su",
            "com.topjohnwu.magisk",
            "com.kingroot.kinguser",
            "com.kingo.root",
            "com.smedialink.oneclickroot",
            "com.zhiqupk.root.global",
            "com.alephzain.framaroot"
        };

        static readonly List<string> KnownDangerousAppsPackages = new List<string> {
            "com.koushikdutta.rommanager",
            "com.koushikdutta.rommanager.license",
            "com.dimonvideo.luckypatcher",
            "com.chelpus.lackypatch",
            "com.ramdroid.appquarantine",
            "com.ramdroid.appquarantinepro",
            "com.android.vending.billing.InAppBillingService.COIN",
            "com.android.vending.billing.InAppBillingService.LUCK",
            "com.chelpus.luckypatcher",
            "com.blackmartalpha",
            "org.blackmart.market",
            "com.allinone.free",
            "com.repodroid.app",
            "org.creeplays.hack",
            "com.baseappfull.fwd",
            "com.zmapp",
            "com.dv.marketmod.installer",
            "org.mobilism.android",
            "com.android.wp.net.log",
            "com.android.camera.update",
            "cc.madkite.freedom",
            "com.solohsu.android.edxp.manager",
            "org.meowcat.edxposed.manager",
            "com.xmodgame",
            "com.cih.game_cih",
            "com.charles.lpoqasert",
            "catch_.me_.if_.you_.can_"
        };

        static readonly List<string> KnownRootCloakingPackages = new List<string> {
            "com.devadvance.rootcloak",
            "com.devadvance.rootcloakplus",
            "de.robv.android.xposed.installer",
            "com.saurik.substrate",
            "com.zachspong.temprootremovejb",
            "com.amphoras.hidemyroot",
            "com.amphoras.hidemyrootadfree",
            "com.formyhm.hiderootPremium",
            "com.formyhm.hideroot"
        };

        // These must end with a /
        static readonly List<string> SuPaths = new List<string> {
            "/data/local/",
            "/data/local/bin/",
            "/data/local/xbin/",
            "/sbin/",
            "/su/bin/",
            "/system/bin/",
            "/system/bin/.ext/",
            "/system/bin/failsafe/",
            "/system/sd/xbin/",
            "/system/usr/we-need-root/",
            "/system/xbin/",
            "/cache/",
            "/data/",
            "/dev/"
        };

        /// <summary>
        /// Return true if the device is rooted/jailbroken
        /// </summary>
        /// <returns></returns>
        public bool IsDeviceRooted()
        {
            if (CheckTestKey()
                || DetectRootManagementApps()
                || DetectRootCloakingApps()
                || DetectPotentiallyDangerousApps()
                || CheckForBinary(BINARY_SU)
                || CheckForBinary(BINARY_BUSYBOX)
                || CheckForBinary(BINARY_MAGISK)
                || CheckSUExists()
                )
            {
                return true;
            }

            return false;
        }

        /*
         * Release-Keys and Test-Keys has to do with how the kernel is signed when it is compiled.
         * Test-Keys means it was signed with a custom key generated by a third-party developer.
         * @return true if signed with Test-keys
         */
        private bool CheckTestKey()
        {
            //check for test key
            var buildTags = Build.Tags;
            if (!string.IsNullOrEmpty(buildTags) && buildTags.Contains("test-keys"))
            {
                return true;
            }

            return false;
        }

        /*
         * Using the PackageManager, check for a list of well known root apps. @link {Const.knownRootAppsPackages}
         * @param additionalRootManagementApps - array of additional packagenames to search for
         * @return true if one of the apps it's installed
         */
        private bool DetectRootManagementApps(params string[] additionalRootManagementApps)
        {
            // Create a list of package names to iterate over from constants any others provided
            if (additionalRootManagementApps != null && additionalRootManagementApps.Length > 0)
            {
                KnownRootAppsPackages.AddRange(additionalRootManagementApps);
            }

            return IsAnyPackageFromListInstalled(KnownRootAppsPackages);
        }

        /*
         * Using the PackageManager, check for a list of well known apps that require root. @link {Const.knownRootAppsPackages}
         * @param additionalDangerousApps - array of additional packagenames to search for
         * @return true if one of the apps it's installed
         */
        private bool DetectPotentiallyDangerousApps(params string[] additionalDangerousApps)
        {

            // Create a list of package names to iterate over from constants any others provided
            if (additionalDangerousApps != null && additionalDangerousApps.Length > 0)
            {
                KnownDangerousAppsPackages.AddRange(additionalDangerousApps);
            }

            return IsAnyPackageFromListInstalled(KnownDangerousAppsPackages);
        }

        /*
         * Using the PackageManager, check for a list of well known root cloak apps. @link {Const.knownRootAppsPackages}
         * @param additionalRootCloakingApps - array of additional packagenames to search for
         * @return true if one of the apps it's installed
         */
        private bool DetectRootCloakingApps(params string[] additionalRootCloakingApps)
        {

            // Create a list of package names to iterate over from constants any others provided
            if (additionalRootCloakingApps != null && additionalRootCloakingApps.Length > 0)
            {
                KnownRootCloakingPackages.AddRange(additionalRootCloakingApps);
            }

            return IsAnyPackageFromListInstalled(KnownRootCloakingPackages);
        }

        /*
         *
         * @param filename - check for this existence of this file
         * @return true if found
         */
        private bool CheckForBinary(string filename)
        {
            foreach (var path in SuPaths)
            {
                if (System.IO.File.Exists(path + filename))
                {
                    return true;
                }
            }

            return false;
        }

        /**
         * A variation on the checking for SU, this attempts a 'which su'
         * @return true if su found
         */
        private bool CheckSUExists()
        {
            //run which su to check if su exists.
            if (CanExecuteCommand("/system/xbin/which su")
                || CanExecuteCommand("/system/bin/which su")
                || CanExecuteCommand("which su"))
            {
                return true;
            }

            return false;
        }

        /*
         * Check if any package in the list is installed
         * @param packages - list of packages to search for
         * @return true if any of the packages are installed
         */
        private bool IsAnyPackageFromListInstalled(List<string> packages)
        {
            PackageManager pm = Application.Context.PackageManager;
            foreach (var packageName in packages)
            {
                try
                {
                    // Root app detected
                    pm.GetPackageInfo(packageName, 0);
                    return true;
                }
                catch (PackageManager.NameNotFoundException)
                {
                    // Exception thrown, package is not installed into the system
                }
            }

            return false;
        }

        //Check if can execute a shell command in Android
        private bool CanExecuteCommand(string command)
        {
            Java.Lang.Process process = null;
            BufferedReader bufferedReader = null;
            try
            {
                process = Runtime.GetRuntime().Exec(command);
                bufferedReader = new BufferedReader(new InputStreamReader(process.InputStream));
                if (bufferedReader.ReadLine() != null)
                {
                    return true;
                }
            }
            catch (System.Exception)
            {
            }
            finally
            {
                bufferedReader?.Close();
                process?.Destroy();
            }

            return false;
        }
    }
}
