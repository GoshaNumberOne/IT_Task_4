using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChessPiecesLibrary.Models
{
    public abstract class ChessPiece : INotifyPropertyChanged
    {
        private string _currentPosition;
        private PieceColor _color;

        public PieceColor Color
        {
            get => _color;
            protected set 
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentPosition
        {
            get => _currentPosition;
            protected set
            {
                if (_currentPosition != value && IsValidBoardPosition(value))
                {
                    _currentPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        protected ChessPiece(PieceColor color, string initialPosition)
        {
            Color = color;
            if (!IsValidBoardPosition(initialPosition))
            {
                throw new ArgumentException("Неверная начальная позиция на доске.", nameof(initialPosition));
            }
            _currentPosition = initialPosition; 
        }

        public bool TryMove(string newPosition)
        {
            if (IsValidBoardPosition(newPosition) && CanMoveTo(CurrentPosition, newPosition))
            {
                CurrentPosition = newPosition; 
                return true;
            }
            return false;
        }
        protected abstract bool CanMoveTo(string currentPosition, string newPosition);

        public static bool IsValidBoardPosition(string position)
        {
            if (string.IsNullOrEmpty(position) || position.Length != 2)
                return false;

            char file = char.ToUpper(position[0]); 
            char rank = position[1];           

            return file >= 'A' && file <= 'H' && rank >= '1' && rank <= '8';
        }

        protected static (int row, int col) AlgebraicToIndices(string position)
        {
            if (!IsValidBoardPosition(position))
                throw new ArgumentException("Неверная строка позиции на доске.", nameof(position));

            int col = char.ToUpper(position[0]) - 'A'; 
            int row = position[1] - '1';            
            return (row, col);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}