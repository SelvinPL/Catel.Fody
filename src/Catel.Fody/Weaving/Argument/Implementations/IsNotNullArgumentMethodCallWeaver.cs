﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsNotNullArgumentMethodCallWeaver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Fody.Weaving.Argument
{
    using System.Linq;

    using Mono.Cecil;

    public sealed class IsNotNullArgumentMethodCallWeaver : DefaultArgumentMethodCallWeaverBase
    {
        protected override void SelectMethod(TypeDefinition argumentTypeDefinition, TypeReference typeToCheck, out MethodDefinition selectedMethod)
        {
            selectedMethod = argumentTypeDefinition.Methods.FirstOrDefault(definition => definition.Name == "IsNotNull" && definition.Parameters.Count == 2 && string.Equals(definition.Parameters[1].ParameterType.FullName, typeToCheck.FullName));
            if (selectedMethod == null)
            {
                selectedMethod = argumentTypeDefinition.Methods.FirstOrDefault(definition => definition.Name == "IsNotNull" && definition.Parameters.Count == 2);
            }
        }
    }
}