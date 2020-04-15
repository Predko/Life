/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 22.11.2016
 * Time: 22:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;


namespace life
{	
	class Program
	{
		[STAThread]
		public static void Main()
		{

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new LifeForm());
		}
	}
}