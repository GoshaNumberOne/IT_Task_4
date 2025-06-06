
namespace ChessPiecesLibrary.Models
{
    public class Queen : ChessPiece 
    {
        public Queen(PieceColor color, string initialPosition) : base(color, initialPosition) { }

        protected override bool CanMoveTo(string currentPosition, string newPosition)
        {
            if (currentPosition == newPosition) return false;

            var (currentRow, currentCol) = AlgebraicToIndices(currentPosition);
            var (newRow, newCol) = AlgebraicToIndices(newPosition);

            bool isHorizontalOrVertical = currentRow == newRow || currentCol == newCol;
            bool isDiagonal = Math.Abs(currentRow - newRow) == Math.Abs(currentCol - newCol);

            return isHorizontalOrVertical || isDiagonal;
        }
    }
}