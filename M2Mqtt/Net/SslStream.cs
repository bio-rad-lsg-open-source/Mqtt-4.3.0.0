using System.Net.Sockets;
using Microsoft.Win32;
using System.Globalization;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Security.Cryptography.X509Certificates;

namespace uPLibrary.Networking.M2Mqtt
{
    /// <summary>
    /// OPEN SSL STREAM
    /// </summary>
    /// 
public class SslStream{

    private const string c_ssleay32DllName = @"D:\CFXSource\depot\Sandbox\Ralph\Cloud\paho.mqtt.m2mqtt-dev\native\windows\x86\ssleay32.dll";
    private const string c_libeay32DllName = @"D:\CFXSource\depot\Sandbox\Ralph\Cloud\paho.mqtt.m2mqtt-dev\native\windows\x86\libeay32.dll";

    private IntPtr ssl ;
    private IntPtr bio ; 
    private IntPtr ctx ;

    private Socket socket;
    
    [DllImport(c_ssleay32DllName)]
    public static extern void SSL_load_error_strings();
    
    [DllImport(c_ssleay32DllName)]
    public static extern int SSL_library_init();
    
    [DllImport(c_libeay32DllName)]
    public static extern void ERR_load_BIO_strings();
    
    [DllImport(c_libeay32DllName)]
    public static extern void OPENSSL_add_all_algorithms_noconf();
    
    [DllImport(c_ssleay32DllName)]
    public static extern void SSL_CTX_free(IntPtr ctx);
    
    [DllImport(c_libeay32DllName)]
    public static extern void BIO_free_all(IntPtr bio);
    
    [DllImport(c_ssleay32DllName)]
    public static extern void SSL_shutdown(IntPtr ssl);
    
    [DllImport(c_ssleay32DllName, EntryPoint="SSLv23_client_method")]
    public static extern IntPtr SSLv23_client_method();
    
    [DllImport(c_ssleay32DllName)]
    public static extern IntPtr SSLv3_method();
    
    [DllImport(c_ssleay32DllName)]
    public static extern IntPtr SSLv23_method();
    
    [DllImport(c_ssleay32DllName)]
    public static extern IntPtr TLSv1_2_method();
    
    [DllImport(c_ssleay32DllName)]
    public static extern IntPtr SSL_CTX_new(IntPtr sslMethod);
    
    [DllImport(c_ssleay32DllName)]
    public static extern IntPtr SSL_new(IntPtr ctx);
    
    [DllImport(c_ssleay32DllName)]
    public static extern int SSL_set_fd(IntPtr ssl, IntPtr fd);
    
    [DllImport(c_ssleay32DllName)]
    public static extern int SSL_connect(IntPtr ssl);
    
    [DllImport(c_ssleay32DllName)]
    public static extern int SSL_get_error(IntPtr ssl, int ret);
    
    [DllImport(c_libeay32DllName)]
    public static extern int ERR_get_error();
    
    [DllImport(c_ssleay32DllName)]
    public static extern int SSL_read(IntPtr ssl, byte[] buffer, int length);
    
    [DllImport(c_ssleay32DllName)]
    public static extern int SSL_write(IntPtr ssl, byte[] buffer, int length);



    /// <summary>
    /// SSL Stream Constructor
    /// </summary>
    /// <param name="socket">Base socket</param>
    public SslStream(Socket socket)
    {
        int i = SSL_library_init();
        SSL_load_error_strings();
        ERR_load_BIO_strings();
        OPENSSL_add_all_algorithms_noconf();
        this.socket = socket;
        ctx = SSL_CTX_new(SSLv23_client_method());
        ssl = SSL_new(ctx);
        SSL_set_fd(ssl, socket.Handle);
        int conStatus = SSL_connect(ssl);
        if (conStatus != 1)
        {
            int errcode = SSL_get_error(ssl, conStatus);
            throw new Exception("SSL handshake error");
        }
    }

    /// <summary>
    /// Read buffer
    /// </summary>
    /// <param name="buffer">Buffer to read</param>
    /// <param name="length">Buffer lenght</param>
    public int Read(ref byte[] buffer, int offset, int length)
    {
        return SSL_read(ssl, buffer,length);
    }

    /// <summary>
    /// Write buffer
    /// </summary>
    /// <param name="buffer">Buffer to read</param>
    /// <param name="length">Buffer lenght</param>
    public int Write( byte[] buffer, int offset, int length)
    {
        return SSL_write(ssl, buffer, buffer.Length);
    }
    /// <summary>
    /// Flush
    /// </summary>
    public int Flush()
    {
        return 0;
    }

    /// <summary>
    /// Close SSL Stream
    /// </summary>
    /// <param name="buffer">Buffer to read</param>
    /// <param name="length">Buffer lenght</param>
    public void Close() {
        SSL_shutdown(ssl);
        BIO_free_all(bio);
        SSL_CTX_free(ctx);
    }
    
    }
}