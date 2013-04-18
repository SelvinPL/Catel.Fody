﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleWeaver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Fody
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Services;

    public class ModuleWeaver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleWeaver"/> class.
        /// </summary>
        /// <remarks>
        /// The class must contain an empty constructor.
        /// </remarks>
        public ModuleWeaver()
        {
            // Init logging delegates to make testing easier
            LogInfo = s => { };
            LogWarning = s => { };
            LogError = s => { };
        }

        /// <summary>
        /// Gets or sets the configuration element. Contains the full element from <c>FodyWeavers.xml</c>.
        /// </summary>
        /// <value>The config.</value>
        public XElement Config { get; set; }

        /// <summary>
        /// Gets or sets the log info delegate.
        /// </summary>
        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public Action<string, SequencePoint> LogWarningPoint { get; set; }

        public Action<string> LogError { get; set; }

        public Action<string, SequencePoint> LogErrorPoint { get; set; }

        /// <summary>
        /// Gets or sets the assembly resolver. Contains a <seealso cref="Mono.Cecil.IAssemblyResolver"/> 
        /// for resolving dependencies.
        /// </summary>
        /// <value>The assembly resolver.</value>
        public IAssemblyResolver AssemblyResolver { get; set; }

        /// <summary>
        /// Gets or sets the module definition. Contains the Cecil representation of the assembly being built.
        /// </summary>
        /// <value>The module definition.</value>
        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            // 1st step: set up the basics
            var msCoreReferenceFinder = new MsCoreReferenceFinder(this, ModuleDefinition.AssemblyResolver);
            msCoreReferenceFinder.Execute();

            var types = ModuleDefinition.GetTypes().Where(x => x.BaseType != null).ToList();

            var typeResolver = new TypeResolver();
            var notifyInterfaceFinder = new CatelModelBaseFinder(typeResolver);
            var typeNodeBuilder = new CatelTypeNodeBuilder(this, notifyInterfaceFinder, typeResolver, types);
            typeNodeBuilder.Execute();

            // 2nd step: Property weaving
            var propertyWeaverService = new PropertyWeaverService(this, typeNodeBuilder, typeResolver, types);
            propertyWeaverService.Execute();

            // 3rd step: Argument weaving
			//TODO
			//var argumentWeaverService = new ArgumentWeaverService();
			//argumentWeaverService.Execute();

            // 4th step: Xml schema weaving
            var xmlSchemasWeaverService = new XmlSchemasWeaverService(this, msCoreReferenceFinder, typeNodeBuilder);
            xmlSchemasWeaverService.Execute();

            // Last step: clean up
            new ReferenceCleaner(this).Execute();
        }
    }
}