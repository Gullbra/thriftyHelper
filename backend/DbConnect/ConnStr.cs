using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftyHelper.Backend.DbConnect;

namespace DbConnect;

internal class ConnStr
{
	public static string Get(bool dbLocal)
	{
		if (dbLocal)
			return string.Format(
				"Host={0};Username={1};Password={2};Database={3}",
				Sectrets.Host,
				Sectrets.Username,
				Sectrets.Password,
				Sectrets.Database);

		var uri = new Uri(Sectrets.HostedConnectionString);
		return string.Format(
			"Server={0};Database={1};User Id={2};Password={3};Port={4}",
			uri.Host,
			uri.AbsolutePath.Trim('/'),
			uri.UserInfo.Split(':')[0],
			uri.UserInfo.Split(':')[1],
			uri.Port > 0 ? uri.Port : 5432);
	}
}
