using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VendorForm.Api.Services;

public static class CryptoHelper
{
    private static byte[] GetDevKey()
    {
        return Encoding.UTF8.GetBytes("27c5b4a2b24a41b583c0d1c81467efa3");
    }

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText)) return string.Empty;

        using var aes = Aes.Create();
        aes.Key = GetDevKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(aes.IV.Concat(cipherBytes).ToArray());
    }

    public static string Decrypt(string encrypted)
    {
        if(string.IsNullOrEmpty(encrypted)) return string.Empty;

        var fullBytes = Convert.FromBase64String(encrypted);

        using var aes = Aes.Create();
        aes.Key = GetDevKey();

        var iv = fullBytes.Take(16).ToArray();
        var cipher = fullBytes.Skip(16).ToArray();

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
