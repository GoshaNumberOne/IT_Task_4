using ChessPiecesLibrary.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ChessApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Queen _whiteQueen;
        private Rook _blackRook;
        private Bishop _whiteBishop;

        private string _targetPositionQueen;
        private string _targetPositionRook;
        private string _targetPositionBishop;
        private string _lastMoveStatus;
        private string _actionLog;

        public Queen WhiteQueen
        {
            get => _whiteQueen;
            set { _whiteQueen = value; OnPropertyChanged(); }
        }

        public Rook BlackRook
        {
            get => _blackRook;
            set { _blackRook = value; OnPropertyChanged(); }
        }

        public Bishop WhiteBishop
        {
            get => _whiteBishop;
            set { _whiteBishop = value; OnPropertyChanged(); }
        }

        public string TargetPositionQueen
        {
            get => _targetPositionQueen;
            set { _targetPositionQueen = value?.ToUpper(); OnPropertyChanged(); }
        }

        public string TargetPositionRook
        {
            get => _targetPositionRook;
            set { _targetPositionRook = value?.ToUpper(); OnPropertyChanged(); }
        }

        public string TargetPositionBishop
        {
            get => _targetPositionBishop;
            set { _targetPositionBishop = value?.ToUpper(); OnPropertyChanged(); }
        }

        public string LastMoveStatus
        {
            get => _lastMoveStatus;
            set { _lastMoveStatus = value; OnPropertyChanged(); }
        }

        public string ActionLog
        {
            get => _actionLog;
            set { _actionLog = value; OnPropertyChanged(); }
        }

        private string _assemblyPath = "ChessPiecesLibrary.dll";
        public string AssemblyPath
        {
            get => _assemblyPath;
            set { _assemblyPath = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TypeDescription> LoadedChessPieceTypes { get; } = new ObservableCollection<TypeDescription>();

        private TypeDescription _selectedChessPieceType;
        public TypeDescription SelectedChessPieceType
        {
            get => _selectedChessPieceType;
            set
            {
                _selectedChessPieceType = value;
                OnPropertyChanged();
                LoadMethodsForSelectedType();
            }
        }

        private PieceColor _constructorPieceColor = PieceColor.White;
        public PieceColor ConstructorPieceColor
        {
            get => _constructorPieceColor;
            set { _constructorPieceColor = value; OnPropertyChanged(); }
        }
        public ObservableCollection<PieceColor> AvailablePieceColors { get; } =
            new ObservableCollection<PieceColor>((PieceColor[])Enum.GetValues(typeof(PieceColor)));


        private string _constructorInitialPosition = "A1";
        public string ConstructorInitialPosition
        {
            get => _constructorInitialPosition;
            set { _constructorInitialPosition = value?.ToUpper(); OnPropertyChanged(); }
        }

        public ObservableCollection<MethodDescription> AvailableMethods { get; } = new ObservableCollection<MethodDescription>();

        private MethodDescription _selectedMethod;
        public MethodDescription SelectedMethod
        {
            get => _selectedMethod;
            set
            {
                _selectedMethod = value;
                OnPropertyChanged();
                LoadParametersForSelectedMethod();
            }
        }

        public ObservableCollection<ParameterInputViewModel> MethodParameters { get; } = new ObservableCollection<ParameterInputViewModel>();

        private string _executionResult;
        public string ExecutionResult
        {
            get => _executionResult;
            set { _executionResult = value; OnPropertyChanged(); }
        }

        public ICommand MovePieceCommand { get; private set; }
        public ICommand ClearLogCommand { get; private set; }
        public ICommand LoadAssemblyCommand { get; private set; }
        public ICommand ExecuteSelectedMethodCommand { get; private set; }


        public MainViewModel()
        {
            WhiteQueen = new Queen(PieceColor.White, "D1");
            BlackRook = new Rook(PieceColor.Black, "A8");
            WhiteBishop = new Bishop(PieceColor.White, "C1");

            ActionLog = string.Empty;
            TargetPositionQueen = "D4";
            TargetPositionRook = "A5";
            TargetPositionBishop = "F4";

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            MovePieceCommand = new RelayCommand(ExecuteMovePiece, CanExecuteMovePiece);
            ClearLogCommand = new RelayCommand(ExecuteClearLog);

            LoadAssemblyCommand = new RelayCommand(ExecuteLoadAssembly);
            ExecuteSelectedMethodCommand = new RelayCommand(ExecuteMethod, CanExecuteMethod);
        }

        private void ExecuteMovePiece(object parameter)
        {
            if (parameter is ChessPiece piece)
            {
                string targetPosition = null;
                string pieceName = GetPieceName(piece);
                string oldPosition = piece.CurrentPosition;

                if (piece is Queen) targetPosition = TargetPositionQueen;
                else if (piece is Rook) targetPosition = TargetPositionRook;
                else if (piece is Bishop) targetPosition = TargetPositionBishop;

                if (string.IsNullOrWhiteSpace(targetPosition))
                {
                    LastMoveStatus = $"{pieceName}: Целевая позиция не указана.";
                    AddToLog($"{pieceName}: Целевая позиция не указана.");
                    return;
                }

                if (!ChessPiece.IsValidBoardPosition(targetPosition))
                {
                    LastMoveStatus = $"{pieceName}: Неверная целевая позиция '{targetPosition}'.";
                    AddToLog($"{pieceName}: Неверная целевая позиция '{targetPosition}'. Используйте формат типа A1-H8.");
                    return;
                }

                if (piece.TryMove(targetPosition))
                {
                    LastMoveStatus = $"{pieceName} перемещен(а) с {oldPosition} на {targetPosition}.";
                    AddToLog($"{pieceName} успешно перемещен(а) с {oldPosition} на {targetPosition}.");
                }
                else
                {
                    LastMoveStatus = $"{pieceName} не может быть перемещен(а) с {oldPosition} на {targetPosition}.";
                    AddToLog($"{pieceName}: Неудачная попытка хода с {oldPosition} на {targetPosition}. Ход невозможен.");
                }
            }
        }

        private bool CanExecuteMovePiece(object parameter)
        {
            return parameter is ChessPiece;
        }

        private void ExecuteClearLog(object parameter)
        {
            ActionLog = string.Empty;
            LastMoveStatus = string.Empty;
            ExecutionResult = string.Empty;
        }

        private void AddToLog(string message)
        {
            ActionLog += $"{DateTime.Now:T}: {message}{Environment.NewLine}";
        }

        private string GetPieceName(ChessPiece piece)
        {
            string colorPrefix = piece.Color == PieceColor.White ? "Белый" : "Черный";
            string pieceType = "Фигура";
            if (piece is Queen) pieceType = "Ферзь";
            else if (piece is Rook) pieceType = "Ладья";
            else if (piece is Bishop) pieceType = "Слон";

            return $"{colorPrefix} {pieceType}";
        }

        private void ExecuteLoadAssembly(object parameter)
        {
            LoadedChessPieceTypes.Clear();
            AvailableMethods.Clear();
            MethodParameters.Clear();
            SelectedChessPieceType = null;
            SelectedMethod = null;
            ExecutionResult = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(AssemblyPath))
                {
                    ExecutionResult = "Путь к сборке не указан.";
                    MessageBox.Show(ExecutionResult, "Ошибка Загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string fullPath = Path.GetFullPath(AssemblyPath);
                if (!File.Exists(fullPath))
                {
                    ExecutionResult = $"Файл сборки не найден: {fullPath}";
                     MessageBox.Show(ExecutionResult, "Ошибка Загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                Assembly loadedAssembly = Assembly.LoadFrom(fullPath);
                Type chessPieceBaseTypeFromDll = loadedAssembly.GetType("ChessPiecesLibrary.Models.ChessPiece");

                if (chessPieceBaseTypeFromDll == null)
                {
                    ExecutionResult = "Базовый тип ChessPiece не найден в загруженной сборке (ожидался ChessPiecesLibrary.Models.ChessPiece).";
                    MessageBox.Show(ExecutionResult, "Ошибка Загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var types = loadedAssembly.GetTypes()
                    .Where(t => t.IsPublic && !t.IsAbstract && chessPieceBaseTypeFromDll.IsAssignableFrom(t))
                    .ToList();

                if (!types.Any())
                {
                     ExecutionResult = "В сборке не найдены подходящие классы, наследующие ChessPiece.";
                }

                foreach (var type in types)
                {
                    LoadedChessPieceTypes.Add(new TypeDescription { Name = type.Name, Type = type });
                }
                 ExecutionResult = $"Сборка {Path.GetFileName(fullPath)} загружена. Найдено классов: {types.Count}.";
            }
            catch (Exception ex)
            {
                ExecutionResult = $"Ошибка загрузки сборки: {ex.Message}";
                MessageBox.Show(ExecutionResult, "Ошибка Загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMethodsForSelectedType()
        {
            AvailableMethods.Clear();
            MethodParameters.Clear();
            SelectedMethod = null;
            ExecutionResult = string.Empty;

            if (SelectedChessPieceType?.Type == null) return;

            var methods = SelectedChessPieceType.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object));

            foreach (var methodInfo in methods)
            {
                AvailableMethods.Add(new MethodDescription(methodInfo));
            }
        }

        private void LoadParametersForSelectedMethod()
        {
            MethodParameters.Clear();
            ExecutionResult = string.Empty;

            if (SelectedMethod?.MethodInfo == null) return;

            foreach (var paramInfo in SelectedMethod.MethodInfo.GetParameters())
            {
                MethodParameters.Add(new ParameterInputViewModel
                {
                    Name = paramInfo.Name,
                    ParameterType = paramInfo.ParameterType,
                    Value = GetDefaultValueForType(paramInfo.ParameterType) 
                });
            }
            System.Diagnostics.Debug.WriteLine($"SelectedMethod: {SelectedMethod?.Name}");
            if (SelectedMethod?.MethodInfo != null)
            {
                foreach (var p in SelectedMethod.MethodInfo.GetParameters())
                {
                    System.Diagnostics.Debug.WriteLine($"Param: {p.Name}, Type: {p.ParameterType.Name}");
                }
            }
            System.Diagnostics.Debug.WriteLine($"MethodParameters count: {MethodParameters.Count}");
        }
         private string GetDefaultValueForType(Type type)
        {
            if (type == typeof(string)) return "";
            if (type == typeof(int) || type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return "0";
            if (type == typeof(bool)) return "false";
            if (type.IsEnum) return Enum.GetNames(type).FirstOrDefault() ?? "";
            return "";
        }


        private bool CanExecuteMethod(object parameter)
        {
            return SelectedChessPieceType != null && SelectedMethod != null;
        }

        private void ExecuteMethod(object parameter)
        {
            if (!CanExecuteMethod(null))
            {
                ExecutionResult = "Выберите класс и метод для выполнения.";
                return;
            }

            try
            {
                if (!ChessPiece.IsValidBoardPosition(ConstructorInitialPosition))
                {
                    ExecutionResult = $"Неверная начальная позиция для конструктора: {ConstructorInitialPosition}";
                    MessageBox.Show(ExecutionResult, "Ошибка Ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                object instance;
                object[] constructorArgs = { ConstructorPieceColor, ConstructorInitialPosition.ToUpper() };
                
                try
                {
                    instance = Activator.CreateInstance(SelectedChessPieceType.Type, constructorArgs);
                }
                catch (MissingMethodException ex)
                {
                    ExecutionResult = $"Не удалось создать экземпляр {SelectedChessPieceType.Name}. Конструктор (PieceColor, string) не найден. {ex.Message}";
                    MessageBox.Show(ExecutionResult, "Ошибка Создания Объекта", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (Exception ex)
                {
                     ExecutionResult = $"Ошибка при создании экземпляра {SelectedChessPieceType.Name}: {ex.Message}";
                     MessageBox.Show(ExecutionResult, "Ошибка Создания Объекта", MessageBoxButton.OK, MessageBoxImage.Error);
                     return;
                }


                object[] methodArgs = new object[MethodParameters.Count];
                for (int i = 0; i < MethodParameters.Count; i++)
                {
                    var paramVM = MethodParameters[i];
                    try
                    {
                        if (paramVM.ParameterType.IsEnum)
                        {
                            methodArgs[i] = Enum.Parse(paramVM.ParameterType, paramVM.Value, true);
                        }
                        else if (paramVM.ParameterType == typeof(bool))
                        {
                            methodArgs[i] = bool.Parse(paramVM.Value);
                        }
                        else
                        {
                            if (SelectedMethod.Name == "TryMove" && paramVM.Name.ToLower().Contains("position"))
                            {
                                if (!ChessPiece.IsValidBoardPosition(paramVM.Value))
                                {
                                     ExecutionResult = $"Неверный формат позиции '{paramVM.Value}' для параметра '{paramVM.Name}'.";
                                     MessageBox.Show(ExecutionResult, "Ошибка Параметра", MessageBoxButton.OK, MessageBoxImage.Error);
                                     return;
                                }
                                methodArgs[i] = paramVM.Value.ToUpper();
                            }
                            else
                            {
                                 methodArgs[i] = Convert.ChangeType(paramVM.Value, paramVM.ParameterType);
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        ExecutionResult = $"Неверный формат для параметра '{paramVM.Name}'. Ожидаемый тип: {paramVM.ParameterType.Name}, Введено: '{paramVM.Value}'.";
                        MessageBox.Show(ExecutionResult, "Ошибка Параметра", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                     catch (ArgumentException)
                    {
                        ExecutionResult = $"Неверное значение '{paramVM.Value}' для перечисления '{paramVM.Name}' (тип: {paramVM.ParameterType.Name}).";
                        MessageBox.Show(ExecutionResult, "Ошибка Параметра", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                object result = SelectedMethod.MethodInfo.Invoke(instance, methodArgs);

                if (SelectedMethod.MethodInfo.ReturnType == typeof(void))
                {
                    ExecutionResult = $"Метод {SelectedMethod.Name} выполнен успешно.";
                }
                else
                {
                    ExecutionResult = $"Результат {SelectedMethod.Name}: {result?.ToString() ?? "null"}";
                }

                AddToLog($"Reflection: {SelectedChessPieceType.Name}.{SelectedMethod.Name} выполнен. Результат: {ExecutionResult}");

            }
            catch (TargetInvocationException ex)
            {
                 ExecutionResult = $"Ошибка выполнения метода {SelectedMethod.Name}: {ex.InnerException?.Message ?? ex.Message}";
                 MessageBox.Show(ExecutionResult, "Ошибка Выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ExecutionResult = $"Общая ошибка: {ex.Message}";
                MessageBox.Show(ExecutionResult, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}