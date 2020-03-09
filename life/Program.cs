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
		public static void Main()
		{

			Application.Run(new LifeForm());


/*
			do 
			{
				Field.OnListCells();

				Console.SetCursorPosition(0,Console.WindowHeight - 1);
				Console.Write("Сделано {0} ходов", ++count);

					keypress = Console.ReadKey(true);
				
				switch (keypress.KeyChar) 
				{
						
					case 'e':
						Console.Clear();
						Field.ClearField();
						Field.EnterCells();
						Field.FieldInit();
						count = 0;
						break;
						
					case 'g':
						
						Field.ClearField();
						
						Field.fld[41,10].SetYes(); Field.fld[41,10].Draw();
						Field.fld[42,10].SetYes(); Field.fld[42,10].Draw();	
						Field.fld[40,11].SetYes(); Field.fld[40,11].Draw();
						Field.fld[41,11].SetYes(); Field.fld[41,11].Draw();
						Field.fld[41,12].SetYes(); Field.fld[41,12].Draw();
						
						Field.FieldInit();
						count = 0;
						
						break;
				}

			} 
			while (keypress.KeyChar != 'q');
*/				
		}
	}
}