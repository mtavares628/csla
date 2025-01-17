﻿//-----------------------------------------------------------------------
// <copyright file="StateController.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Gets and puts the current user session data</summary>
//-----------------------------------------------------------------------
using System.IO;
using Csla.Serialization.Mobile;
using Microsoft.AspNetCore.Mvc;
using Csla.State;

namespace Csla.AspNetCore.Blazor.State
{
  /// <summary>
  /// Gets and puts the current user session data
  /// from the Blazor wasm client components.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="sessionManager"></param>
  [ApiController]
  [Route("[controller]")]
  public class StateController(ApplicationContext applicationContext, ISessionManager sessionManager) : ControllerBase
  {
    private readonly ApplicationContext ApplicationContext = applicationContext;
    private readonly ISessionManager _sessionManager = sessionManager;

    /// <summary>
    /// Gets current user session data in a serialized
    /// format.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public virtual byte[] Get()
    {
      var session = _sessionManager.GetSession();
      session.IsCheckedOut = true;
      var formatter = new MobileFormatter(ApplicationContext);
      var buffer = new MemoryStream();
      formatter.Serialize(buffer, session);
      return buffer.ToArray();
    }

    /// <summary>
    /// Sets the current user session data from a
    /// serialized format.
    /// </summary>
    /// <param name="updatedSessionData"></param>
    /// <returns></returns>
    [HttpPut]
    public virtual void Put(byte[] updatedSessionData)
    {
      var formatter = new MobileFormatter(ApplicationContext);
      var buffer = new MemoryStream(updatedSessionData)
      {
        Position = 0
      };
      var session = (Session)formatter.Deserialize(buffer);
      session.IsCheckedOut = false;
      _sessionManager.UpdateSession(session);
    }

    /// <summary>
    /// Sets the current user session data as checked out,
    /// indicating that it is in use by a Blazor wasm client.
    /// </summary>
    [HttpPatch]
    public virtual void Patch()
    {
      var session = _sessionManager.GetSession();
      session.IsCheckedOut = true;
    }
  }
}
