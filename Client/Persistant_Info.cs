using Microsoft.AspNetCore.Components;
using Hermes.Shared;
using System.Net.Http;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace Hermes.Client
{
 public class Persistant_Info
 {
  private string? _unit;
  public string? Unit
  {
   get
   {
    return _unit;
   }
  }

  private string? _password;
  public string? Password
  {
   get
   {
    return _password;
   }
  }

  private DateTime? _expires;
  private ILocalStorageService _localStorageService;
  private HttpClient _httpClient;
  private NavigationManager _navigationManager;

  public Persistant_Info(NavigationManager nav, HttpClient http, ILocalStorageService localStorageService)
  {

   _httpClient = http;
   _navigationManager = nav;
   _localStorageService = localStorageService;
   var x = Task.Run<string?>(async () => { return await _localStorageService.GetItemAsync<string?>("Unit").AsTask(); }).ContinueWith(async a =>
   {
    string? t = a.Result;
    if (t != null)
     _unit = t;
   });
   var y = Task.Run<string?>(async () => { return await _localStorageService.GetItemAsync<string?>("Password").AsTask(); }).ContinueWith(async a =>
   {
    string? t = a.Result;
    if (t != null)
     _password = t;
   });
   var z = Task.Run<DateTime?>(async () => { return await _localStorageService.GetItemAsync<DateTime?>("Expiry").AsTask(); }).ContinueWith(async a =>
   {
    DateTime? t = a.Result;
    if (t != null)
     _expires = t;
   });
  }

  private async Task<List<Assignment>?> getCurrentAssignments_priv(string? username, string? password, GeolocationCoordinates? glc)
  {
   if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    return null;
   try
   {
    if (glc != null)
     return await _httpClient.GetFromJsonAsync<List<Assignment>?>($"API/CurrentAssignments/g/{username}/{password}/{glc.Latitude}/{glc.Longitude}");
    else
     return await _httpClient.GetFromJsonAsync<List<Assignment>?>($"API/CurrentAssignments/g/{username}/{password}/X/X");
   }
   catch (Exception ex) { return null; }
  }

  private async Task<List<Priority>?> getCurrentPriorities_priv(string? username, string? password, GeolocationCoordinates? glc)
  {
   if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    return null;
   try
   {
    if (glc != null)
     return await _httpClient.GetFromJsonAsync<List<Priority>?>($"API/CurrentAssignments/getprios/{username}/{password}");
    else
     return await _httpClient.GetFromJsonAsync<List<Priority>?>($"API/CurrentAssignments/getprios/{username}/{password}");
   }
   catch (Exception ex) { return null; }
  }

  public async Task<List<Assignment>?> getCurrentAssignments(GeolocationCoordinates? glc)
  {
   return await getCurrentAssignments_priv(_unit, _password, glc);
  }

  public async Task<List<Priority>?> getCurrentPriorities(GeolocationCoordinates? glc)
  {
   return await getCurrentPriorities_priv(_unit, _password, glc);
  }

  public async Task ForceInit()
  {
   _expires = await _localStorageService.GetItemAsync<DateTime?>("Expiry");

   if (_expires != null && _expires < DateTime.UtcNow)
    return;

   _unit = await _localStorageService.GetItemAsync<string?>("Unit");
   _password = await _localStorageService.GetItemAsync<string?>("Password");

  }

  public async Task<bool> validate(string u, string p)
  {
   if (u == null || p == null) return false;
   if (string.IsNullOrEmpty(u)
       || string.IsNullOrEmpty(p)) return false;

   if (await getCurrentAssignments_priv(u, p, null) == null)
    return false;


   _unit = u;
   _password = p;
   _expires = DateTime.UtcNow.AddDays(1);

   await _localStorageService.SetItemAsync<string>("Unit", _unit);
   await _localStorageService.SetItemAsync<string>("Password", _password);
   await _localStorageService.SetItemAsync<DateTime?>("Expiry", _expires);

   return true;
  }

 }
}
