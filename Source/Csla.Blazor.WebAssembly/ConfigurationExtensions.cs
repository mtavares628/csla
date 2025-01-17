﻿//-----------------------------------------------------------------------
// <copyright file="BlazorWasmConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Csla.State;
using Csla.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Csla.Blazor.WebAssembly.State;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for Blazor WebAssembly
  /// </summary>
  public static class BlazorWasmConfigurationExtensions
  {
    /// <summary>
    /// Registers services necessary for Blazor WebAssembly.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <returns></returns>
    public static CslaOptions AddBlazorWebAssembly(this CslaOptions config)
    {
      return AddBlazorWebAssembly(config, null);
    }

    /// <summary>
    /// Registers services necessary for Blazor WebAssembly.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <param name="options">Options object</param>
    /// <returns></returns>
    public static CslaOptions AddBlazorWebAssembly(this CslaOptions config, Action<BlazorWebAssemblyConfigurationOptions> options)
    {
      var blazorOptions = new BlazorWebAssemblyConfigurationOptions();
      options?.Invoke(blazorOptions);

      config.Services.AddScoped((p) => blazorOptions);
      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      config.Services.TryAddScoped<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
      config.Services.TryAddScoped<IAuthorizationHandler, CslaPermissionsHandler>();
      config.Services.TryAddScoped(typeof(Csla.Core.IContextManager), typeof(Csla.Blazor.WebAssembly.ApplicationContextManager));
      config.Services.TryAddScoped(typeof(AuthenticationStateProvider), typeof(Csla.Blazor.Authentication.CslaAuthenticationStateProvider));

      // use Blazor state management
      config.Services.AddScoped(typeof(ISessionManager), blazorOptions.SessionManagerType);
      config.Services.AddTransient<Blazor.State.StateManager>();

      return config;
    }
  }

  /// <summary>
  /// Options for Blazor wasm-interactive.
  /// </summary>
  public class BlazorWebAssemblyConfigurationOptions
  {
    /// <summary>
    /// Gets or sets the type of the ISessionManager service.
    /// </summary>
    public Type SessionManagerType { get; set; } = typeof(SessionManager);

    /// <summary>
    /// Gets or sets the name of the controller providing state
    /// data from the Blazor web server.
    /// </summary>
    public string StateControllerName { get; set; } = "CslaState";
    /// <summary>
    /// Gets or sets a value indicating whether the LocalContext
    /// and ClientContext values on ApplicationContext should be
    /// synchronized with a Blazor web server host.
    /// </summary>
    /// <remarks>
    /// If this value is true, the Blazor web server host must
    /// provide a state controller to allow the wasm client to
    /// get and send the state from/to the server as necessary.
    /// </remarks>
    public bool SyncContextWithServer {  get; set; } = false;
  }
}