/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 22.11.2016
 * Time: 22:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;


namespace life
{
	
	// кординаты ячеёки
	public struct Coord {
		public int	x,y;
	}
	
	// отрисовка клетки
	interface IDraw {
		void Draw();
	}
	
	
	public enum Calccmd {set_status, calc_cell, del_calc_func};
	public enum Cell_is {yes, no};

		
	public class  Cell: IDraw  {
		public char		C {get; private set;}	// символ L  или E - есть клетка или нет
		public bool		gen;				// поколение клетки: true - первое, false - нулевое
		public bool		ischange;		// изменение клетки: true - должно измениться, false - нет
		public bool 	active;			// активная ячейка(true)- добавлена в список событий
		public int		X {get; set;}			// координаты ячейки
		public int		Y {get; set;}           // координаты ячейки

		public const char L = '0';	// Клетка есть
		public const char E = ' ';  // Клетки нет

		private readonly Field Field;
		
		public bool	IsLive() {
			return gen && C == L; // клетка первого поколения(не только что созданная) и клетка живая
		}
		

		public Cell(Field fld, int px = 0, int py = 0) 
		{
			Field = fld;
			SetCell();
			X = px; Y = py;
		}

		
		public void Copy(Cell cl) {
			SetCell(cl.C);
			X = cl.X; Y = cl.Y;
		}
		
		public void SetCell(char cl = E) {
			C = cl;
			gen = true;
			ischange = false;
			active = false;
		}
		
		public void SetNo() {
			SetCell();
		}
		
		public void SetYes() {
			SetCell(L);
		}
		
		// отрисовываем ячейку
		public virtual void Draw() {
			Console.SetCursorPosition(X,Y);
			Console.Write(C);
		}
		
		// рассчитываем следующий ход ячейки(жива/нет) или меняем статус клетки если cng == true,
		// удаляется из списка событий, если нет,
		// удаляется из списка событий, если рядом нет живых ячеек первого поколения,
		// если рядом больше двух живых ячеек первого поколения, отмечает себя живой, проверяем соседние
		public void Calc(Calccmd cng){
			
			if (cng == Calccmd.set_status) {
				if (ischange) {
					if (C == E) {
						C = L;							// клетка появится
						Field.IsLiveCount(X, Y, IsLive());  // добавляем окружающие в список события
					}
					else
						C = E;							// клетка исчезнет
					
					Draw();
					ischange = false;
				}
				return;
			}
			else if (cng == Calccmd.del_calc_func) {
				Field.ListCells -= Calc;
				return;
			}
			
			
			int countcells = Field.IsLiveCount(X, Y, IsLive());
			
			if (countcells == 3) {
				if (C == E)			// клетки нет
					ischange =  true;	// клетка рождается
			}
			else if (countcells != 2) {
				    
				if (C == L)		  	// клетка есть
					ischange = true; 	// клетка исчезает 
				else
					if (countcells == 0) { // клетки нет и вокруг нет живых - удаляем из списка события
						Field.ListCells -= Calc;
						active = false;
					}
			}
		}
	}
	
	// Индексатор с круговыми индексами
	
	public class CellArray {
		readonly Cell[,]	cells;
		private readonly int mi;
		private readonly int mj;

		

		public CellArray(Field fld, int i, int j)  
		{
			cells = new Cell[i, j];
			// инициализация всех ячеек
			for (int k = 0; k != i; k++)
				for (int m = 0; m != j; m++)
					cells[k,m] = new Cell(fld, k, m);
				
			mi = i;
			mj = j;
		}
		
		// индексатор
		public Cell this[int i, int j] {
			get {
				return cells[RetIndexI(i),RetIndexJ(j)];
			}
			set {
				cells[RetIndexI(i),RetIndexJ(j)].Copy(value);
			}
		}
			
		// возвращает правильный индекс i = 0 - (mi-1)
		int RetIndexI(int i) {
			if (i < 0) return mi + i;
			else if (i >= mi) return i - mi;
			
			return i;
		}
		
		// возвращает правильный индекс j = 0 - (mj-1)
		int RetIndexJ(int j) {
			if (j < 0) return mj + j;
			else if (j >= mj) return j - mj;
			
			return j;
		}
		
		
	}
	
	public delegate void CalcCell(Calccmd cng);
	
	// поле, верхняя часть соединена с нижней, левая с правой
	public class Field {
		private readonly int 	length;
		private readonly int 	width;

		// массив координат ячеек вокруг данной
		private readonly Coord[] Cxy = new Coord[8];
		
		public CellArray fld;

		
		public event CalcCell ListCells;
		
		
		public Field() {
			length = Console.WindowHeight - 1;
			width = Console.WindowWidth ;
			fld = new CellArray(this, width, length);
			Cxy[0].x = -1; Cxy[0].y = -1; Cxy[1].x = -1; Cxy[1].y = 0; Cxy[2].x = -1; Cxy[2].y = 1;
			Cxy[3].x = 0; Cxy[3].y = -1; Cxy[4].x = 0; Cxy[4].y = 1;
			Cxy[5].x = 1; Cxy[5].y = -1; Cxy[6].x = 1; Cxy[6].y = 0; Cxy[7].x = 1; Cxy[7].y = 1;
			
			EnterCells();
			
			FieldInit();
		}
		
		public void FieldInit() {
			
			for(int x = 0; x != width; x++)
				for(int y = 0; y != length; y++) {
					fld[x,y].Draw();
					if (fld[x,y].C == Cell.L || IsAddlist(x,y)){
						ListCells += fld[x,y].Calc;
						fld[x,y].active = true;
					}
				}
		}

		public void ClearField() {

			for(int x = 0; x != width; x++)
				for(int y = 0; y != length; y++) {
					fld[x,y].SetNo();
					fld[x,y].Draw();
			}
			
			ClearListCells();
		}





		// ввод живых ячеек с помощью мыши
		
		
		public void EnterCells() {

#region заполняем случайным образом (пробный вариант)
			Random rnd = new Random();
			
			for(int x = 0; x != width; x++)
				for(int y = 0; y != length; y++) {
					
					int r = rnd.Next(300);
					if (r > 100 && r < 200)
//					    && x > 40 && x < 60
//					    && y > 8  && y < 12)
						fld[x,y].SetYes();	// клетка есть (живая)
					else
						fld[x,y].SetNo() ;	// клетки нет
				}
#endregion
				
//			for(int x = 0; x != width; x++)
//				for(int y = 0; y != length; y++) 
//					fld[x,y].SetNo() ;	// клетки нет
//
//			// вводим 4 клетки "квадрат"
//			fld[0,0].SetYes();	
//			fld[1,0].SetYes();	
//			fld[0,1].SetYes();	
//			fld[1,1].SetYes();	
				
		}


		
		// проверяем, находится ли рядом с данной ячейкой, хотя бы одна живая ячейка.
		public bool IsAddlist(int x, int y){
		
			foreach(Coord i in Cxy)
				if (fld[x + i.x, y + i.y].IsLive()) return true;

			return false;
		}
		
		// подсчёт живых ячеек вокруг данной. status - состояние вызывающей ячейки: true - live, false - dead
		public int IsLiveCount(int x, int y, bool status) {

			int count = 0;   // счётчик живых ячеек первого поколения вокруг данной

			
			foreach(Coord i in Cxy) {
				
				if (status)  							  // если вызывающая клетка живая
					if (!fld[x + i.x, y + i.y].active) {  // добавляем найденную в список событий, если ещё не добавлена
						ListCells += fld[x + i.x, y + i.y].Calc;
						fld[x + i.x, y + i.y].active = true;
					}

				if (fld[x + i.x, y + i.y].IsLive())		// если найденная клетка живая - увеличиваем счётчик
					count++;
				
			}
			
			return count;
		}
		
		public void OnListCells() {
			
			if (ListCells != null) {
				
					ListCells (Calccmd.set_status);	// фиксируем изменения прошлого хода
					ListCells (Calccmd.calc_cell);	// делаем новый ход (вычисляем)
					
			}
		}

		public void ClearListCells() {

			ListCells?.Invoke(Calccmd.del_calc_func);   // удаляем все обработчики события
		}
		
		
	}
	
	
	
	
	
	
	class Program
	{
		public static void Main(string[] args)
		{
			ConsoleKeyInfo keypress;
			Field Field;
			int count = 0;
			
			Console.Clear();

			Field = new Field();

			do {
				Field.OnListCells();

				Console.SetCursorPosition(0,Console.WindowHeight - 1);
				Console.Write("Сделано {0} ходов", ++count);

					keypress = Console.ReadKey(true);
				
				switch (keypress.KeyChar) {
						
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
				
				 
				
					
				
			} while (keypress.KeyChar != 'q');
				
		}
	}
}