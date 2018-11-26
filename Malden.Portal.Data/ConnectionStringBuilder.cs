namespace Malden.Portal.Data.TableStorage
{
    public static class ConnectionStringBuilder
    {
        public static string GetConnectionString
        {
            get
            {
#if (DEBUG)
                {
                   //return Properties.Settings.Default.PrivatePortalConnectionString;
                   //return Properties.Settings.Default.LiveConnectionString;
                   return Properties.Settings.Default.DebugConnectionString;
                }
#else
                {
                    return Properties.Settings.Default.LiveConnectionString;
                }
#endif
            }
        }

        public static string GetCDNEndPoint
        {
            get
            {
#if (DEBUG)
                {
                    //return Properties.Settings.Default.PrivatePortalCDNEndPoint;
                    return Properties.Settings.Default.LiveCDNEndPoint;
                }
#else
                {
                    return Properties.Settings.Default.LiveCDNEndPoint;
                }
#endif
            }
        }
    }
}