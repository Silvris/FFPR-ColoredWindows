using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;

namespace FFPR_ColoredWindows
{
    //shamelessly copied from Memoria.FFPR, found here: https://github.com/Albeoris/Memoria.FFPR
    public sealed class TypeRegister
    {
        private readonly ManualLogSource _log;

        public TypeRegister(ManualLogSource logSource)
        {
            _log = logSource ?? throw new ArgumentNullException(nameof(logSource));
        }

        public void RegisterRequiredTypes()
        {


            try
            {
                // Not supported :(

                // _log.LogInfo("Registering required types...");
                //
                // ClassInjector.RegisterTypeInIl2Cpp<Dictionary<String, IntPtr>>();
                //
                // _log.LogInfo($"1 additional types required successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register required types.", ex);
            }
        }

        public void RegisterAssemblyTypes()
        {
            try
            {
                _log.LogInfo("Registering assembly types...");

                MethodInfo registrator = typeof(ClassInjector).GetMethod("RegisterTypeInIl2Cpp", new Type[0]);
                if (registrator == null)
                    throw new Exception("Cannot find method RegisterTypeInIl2Cpp.");

                Assembly assembly = Assembly.GetExecutingAssembly();
                Int32 count = RegisterTypes(assembly, registrator);

                _log.LogInfo($"{count} assembly types registered successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register assembly types.", ex);
            }
        }

        private Int32 RegisterTypes(Assembly assembly, MethodInfo registrator)
        {
            Int32 count = 0;
            var parameters = new object[0];

            foreach (Type type in assembly.GetTypes())
            {
                _log.LogInfo(type.FullName);
                if (!IsImportableType(type))
                {
                    continue;
                }
                _log.LogInfo(type == null);
                ClassInjector.RegisterTypeInIl2Cpp(type);
                count++;
                _log.LogInfo("count++");
            }

            return count;
        }

        private static Boolean IsImportableType(Type type)
        {

            return type.Namespace?.EndsWith(".IL2CPP") ?? false;
        }
    }
}
