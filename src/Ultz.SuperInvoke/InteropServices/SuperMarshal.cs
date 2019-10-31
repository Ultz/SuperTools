using System;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace Ultz.SuperInvoke.InteropServices
{
    /// <summary>
    /// Contains methods for marshalling managed types to native types. Generally, these are just proxy methods to
    /// the System.Marshal methods, but we have to declare them within SuperInvoke so that Cecil doesn't play up.
    /// </summary>
    public static class SuperMarshal
    {
        public static unsafe char* ToBStr(string str)
        {
            return (char*) Marshal.StringToBSTR(str);
        }

        public static unsafe char* ToHGlobalUni(string str)
        {
            return (char*) Marshal.StringToHGlobalUni(str);
        }

        public static unsafe char* ToHGlobalAnsi(string str)
        {
            return (char*) Marshal.StringToHGlobalAnsi(str);
        }

        public static unsafe char* ToHGlobalAuto(string str)
        {
            return (char*) Marshal.StringToHGlobalAuto(str);
        }

        public static unsafe string ToStringUni(char* str)
        {
            return Marshal.PtrToStringUni((IntPtr) str);
        }

        public static unsafe string ToStringAnsi(char* str)
        {
            return Marshal.PtrToStringAnsi((IntPtr) str);
        }

        public static unsafe string ToStringAuto(char* str)
        {
            return Marshal.PtrToStringAuto((IntPtr) str);
        }

        public static unsafe string ToStringBStr(char* str)
        {
            return Marshal.PtrToStringBSTR((IntPtr) str);
        }

        public static unsafe void FreeHGlobal(char* str)
        {
            Marshal.FreeHGlobal((IntPtr) str);
        }

        public static unsafe void FreeBStr(char* str)
        {
            Marshal.FreeBSTR((IntPtr) str);
        }

        public static unsafe char* AllocBStr(int len)
        {
            return (char*) Marshal.StringToBSTR(new string('\0', len));
        }

        public static unsafe char* AllocHGlobalString(int len)
        {
            return (char*) Marshal.AllocHGlobal(len);
        }
    }
}