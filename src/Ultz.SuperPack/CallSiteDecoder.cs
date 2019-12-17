using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace Ultz.SuperPack
{
    struct CallSiteDecoder
    {
	    public CallSiteDecoder(ByteBuffer data)
	    {
	    }

	    public static CallSite Decode(ByteBuffer data)
	    {
		    var callingConvention = (MethodCallingConvention)data.ReadByte();
		    int parameterCount = (int)data.ReadCompressedUInt32();
		    bool isOptional = false;
		    var ret = DecodeType(ref isOptional);
		    ret.AssemblyQualifiedName

		    if (ret == null)
			    throw new ArgumentException(nameof(data));

		    if (!DecodeParameters(parameterCount))
			    throw new ArgumentException(nameof(data));
	    }
        		// Decode a Class or ValueType token from the signature data
		private bool DecodeToken(ElementType elementType, ref int offset, out Type type)
		{
			if (elementType == ElementType.Class || elementType == ElementType.ValueType)
			{
				try
				{
					type = Parent.ResolveType(Data.ReadCompressedTypeDefOrRef(offset, out int count));
					if (type != null)
					{
						offset += count;
						return true;
					}
				}
				catch
				{
				}
			}

			type = null;
			return false;
		}

		// Decode a Type from the signature data
		private static Type DecodeType(ByteBuffer data, ref bool isOptional)
		{
			var elementType = (ElementType)data.ReadByte();

			if (elementType == ElementType.Sentinel)
			{
				elementType = (ElementType)Data.ReadByte(offset++);
				isOptional = true;
			}

			if (!DecodeToken(elementType, out var type))
				type = elementType.ToType();
			return type;
		}

		// Decode the parameters
		private bool DecodeParameters(int offset, int parameterCount)
		{
			var requiredParameters = new List<Type>();
			var optionalParameters = new List<Type>();
			var isOptional = false;

			for(var index = 0; index < parameterCount; index++)
			{
				var type = DecodeType(offset, ref isOptional, out var count);
				if (type == null)
					return false;

				if (isOptional)
					optionalParameters.Add(type);
				else
					requiredParameters.Add(type);

				offset += count;
			}

			RequiredParameters = new ReadOnlyCollection<Type>(requiredParameters);
			OptionalParameters = new ReadOnlyCollection<Type>(optionalParameters);
			return true;
		}

		public ReadOnlyCollection<Type> RequiredParameters { get; set; }

		public ReadOnlyCollection<Type> OptionalParameters { get; set; }

		public MethodCallingConvention CallingConvention { get; set; }
    }
}