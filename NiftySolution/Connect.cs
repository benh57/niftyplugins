// Copyright (C) 2006-2007 Jim Tilander. See COPYING for and README for more details.
using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

// Setup stuff taken from: http://blogs.msdn.com/jim_glass/archive/2005/08/18/453218.aspx
namespace Aurora
{
	namespace NiftySolution
	{
		// This class is the required interface to Visual Studio. 
		// Simple a very lightweight wrapper around the plugin object.
		public class Connect : IDTExtensibility2, IDTCommandTarget
		{
			private Plugin m_plugin;
			private SolutionBuildTimings m_timings;
			
			public Connect()
			{
			}

			public void OnConnection(object application_, ext_ConnectMode connectMode, object addInInst, ref Array custom)
			{
				if( null != m_plugin )
					return;
				
				DTE2 application = (DTE2)application_;
				m_plugin = new Plugin(application, (AddIn)addInInst, "NiftySolution", "Aurora.NiftySolution.Connect");

				m_plugin.RegisterCommand("NiftyOpen", new QuickOpen() );
				m_plugin.RegisterCommand("NiftyToggle", new ToggleFile() );
				m_plugin.AddConsoleOnlyCommand("NiftyOpen", "Open Dialog", "Quickly open any file in the solution");
				m_plugin.AddConsoleOnlyCommand("NiftyToggle", "Toggle .h/.cpp", "Quickly go between .cpp and .h file");
						
				m_timings = new SolutionBuildTimings(application);
			}

			public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
			{
			}

			public void OnAddInsUpdate(ref Array custom)
			{
			}

			public void OnStartupComplete(ref Array custom)
			{
			}

			public void OnBeginShutdown(ref Array custom)
			{
			}

			public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
			{
				if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone &&
					m_plugin.CanHandleCommand(commandName))
				{
					if (m_plugin.IsCommandEnabled(commandName))
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					else
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported;
				}
			}

			public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
			{
				handled = false;
				if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault)
					return;

				handled = m_plugin.OnCommand(commandName);
			}
		}
	}

}