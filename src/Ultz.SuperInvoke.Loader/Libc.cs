﻿using System;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.Loader
{
    internal static class Libc
    {
        private const string LibName = "libc";

        public const int RtldNow = 0x002;

        [DllImport(LibName)]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibName)]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibName)]
        public static extern int dlclose(IntPtr handle);

        [DllImport(LibName)]
        public static extern string dlerror();
    }
}