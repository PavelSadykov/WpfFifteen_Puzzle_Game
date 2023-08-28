using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Controls;

namespace WpfFifteen_Puzzle_Game

{

    public partial class MainWindow : Window

    {

        private ObservableCollection<int> numbers;//коллекция чисел (плиток) на игровом поле

        private Random random = new Random();//объект для перемешивания плиток



        public ICommand MoveCommand { get; }//команда для перемещения плиток

        public ICommand ShuffleCommand { get; }//команда для перемешивания плиток



        public MainWindow()//конструктор окна

        {

            InitializeComponent();



            numbers = new ObservableCollection<int>(Enumerable.Range(1, 15));//создаем коллекцию

            numbers.Add(0); //добавляем в коллекцию пустую клетку

            MoveCommand = new RelayCommand<object>(Move);//создаем экземпляр комманды перемещения

            ShuffleCommand = new RelayCommand<object>(Shuffle);//создаем экземпляр комманды перемешивания

            board.ItemsSource = numbers;//связываем коллекцию numbers с элементом управления
            //ItemsControl с именем board

            this.DataContext = this;//свойство для привязки данных
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                var number = (int)button.Content;
                Move(number);
            }
        }

        private void Move(object parameter)//метод обработки перемещения плитки
        {
            int index = numbers.IndexOf((int)parameter);//получаем индекс выбраной плитки из коллекции
            int emptyIndex = numbers.IndexOf(0);//получаем индекс пустой плитки

            if (IsAdjacent(index, emptyIndex))//соседние ли плитки
            {
                numbers[emptyIndex] = (int)parameter;//перемещение плитки на пустое место
                numbers[index] = 0;
                UpdateTilePositions();//вызов метода обновления позиций

                if (IsGameSolved())
                {
                    MessageBox.Show("Поздравляем! Вы решили головоломку!");
                }
            }
        }
        private bool IsGameSolved()//метод проверки решена ли головоломка
        {
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                if (numbers[i] != i + 1)
                {
                    return false;
                }
            }
            return true;
        }
        private void UpdateTilePositions()//метод обновления позиции клеток
        {
            int rowIndex = 0;
            int colIndex = 0;

            foreach (var number in numbers)
            {
                //Получаем  объект типа ContentPresenter, который содержит элемент (плитку)
                //с определенным числовым значением.
                //Преобразовываем этот контейнер в объект типа Button:
                var button = board.ItemContainerGenerator.ContainerFromItem(number) as Button;
                if (button != null)//Удалось ли преобразовать контейнер в объект  кнопка
                {
                    //Если да, устанавливаем значение сторк и столбцов в сетке Grid для кнопки.
                    //Это и определяет позицию на игровом поле
                    Grid.SetRow(button, rowIndex);
                    Grid.SetColumn(button, colIndex);
                }

                colIndex++;
                if (colIndex >= 4)
                {
                    colIndex = 0;
                    rowIndex++;
                }
            }
        }

        private void Shuffle(object parameter)//метод перемешивания плиток

        {
            //создаем массив чисел из коллекции numbers , рандомно отсортированный:
            int[] shuffledNumbers = numbers.OrderBy(n => random.Next()).ToArray();

            for (int i = 0; i < shuffledNumbers.Length; i++)

            {

                numbers[i] = shuffledNumbers[i];

            }

        }



        private bool IsAdjacent(int index1, int index2)//метод для проверки соседние ли две плитки 

        {

            int row1 = index1 / 4;//определяем  в каком ряду находится плитка

            int col1 = index1 % 4;// определяем  в какой строке находится плитка

            int row2 = index2 / 4;

            int col2 = index2 % 4;


            //вычисляем сумму модулей разницы между row и col. Если она = 1 , то плитки рядом
            return Math.Abs(row1 - row2) + Math.Abs(col1 - col2) == 1;

        }

    }



    public class RelayCommand<T> : ICommand//реализация интерфейса ICommand
    {
        private readonly Action<T> _execute;// объявляем приватное поле _execute (делегат типа Action<T>)

        public RelayCommand(Action<T> execute)//конструктор класса,принимающийй метод
                                              //_execute котррый будет выполняться при выполнении команды

        {

            _execute = execute;

        }

        public bool CanExecute(object parameter) => true;//метод возможности выполнения команды

        public void Execute(object parameter) => _execute((T)parameter);//метод выполнения заданного метода _execute

        public event EventHandler CanExecuteChanged { add { } remove { } }//событие , кот сигнализирует об изменении возможности выполнения команды



    }

}
