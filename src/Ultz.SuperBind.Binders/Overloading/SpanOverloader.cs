//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Ultz.SuperBind.Core;
//
//namespace Ultz.SuperBind.Binders.Overloading
//{
//    public class SpanOverloader : RecursiveOverloaderBase
//    {
//        public override bool IsApplicable(MethodSpecification method) => method.Parameters.Any(IsApplicable);
//        private bool IsApplicable(ParameterSpecification arg)
//        {
//            if (!arg.TempData.ContainsKey("KHR_LEN"))
//            {
//                return false;
//            }
//
//            var len = arg.TempData["KHR_LEN"];
//            return !int.TryParse(len, out _) && arg.Type.PointerLevels > 0;
//        }
//
//        protected override MethodSpecification CreateOne(MethodSpecification method)
//        {
//            for (var i = 0; i < method.Parameters.Length; i++)
//            {
//                var param = method.Parameters[i];
//                var ret = (MethodSpecification) method.Clone();
//                ret.Parameters[i] = new ParameterSpecification
//                {
//                    
//                }
//            }
//
//            throw new InvalidOperationException("Overloader not applicable.");
//        }
//    }
//}