﻿using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const string _httpServerAddress = "https://projectserveraapi.azurewebsites.net";
    public string HttpServerAddress
    {
        get
        {
            return _httpServerAddress;
        }
    }

    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    private string _id;
    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    private string _nickname;
    public string Nickname
    {
        get { return _nickname; }
        set { _nickname = value; }
    }

    private DateTime _birthday;
    public DateTime BirthDay
    {
        get { return _birthday; }
        set { _birthday = value; }
    }

    private string _email;
    public string Email
    {
        get { return _email; }
        set { _email = value; }
    }

    private string _country;
    public string Country
    {
        get { return _country; }
        set { _country = value; }
    }

    private string _bloburi;
    public string BlobUri
    {
        get { return _bloburi; }
        set { _bloburi = value; }
    }
}
