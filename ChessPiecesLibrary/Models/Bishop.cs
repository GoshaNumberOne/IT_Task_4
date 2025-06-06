
namespace ChessPiecesLibrary.Models
{
    public class Bishop : ChessPiece
    {
        public Bishop(PieceColor color, string initialPosition) : base(color, initialPosition) { }

        protected override bool CanMoveTo(string currentPosition, string newPosition)
        {
            if (currentPosition == newPosition) return false;

            var (currentRow, currentCol) = AlgebraicToIndices(currentPosition);
            var (newRow, newCol) = AlgebraicToIndices(newPosition);

            return Math.Abs(currentRow - newRow) == Math.Abs(currentCol - newCol);
        }
    }
}