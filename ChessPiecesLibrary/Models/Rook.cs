
namespace ChessPiecesLibrary.Models
{
    public class Rook : ChessPiece 
    {
        public Rook(PieceColor color, string initialPosition) : base(color, initialPosition) { }

        protected override bool CanMoveTo(string currentPosition, string newPosition)
        {
            if (currentPosition == newPosition) return false; 

            var (currentRow, currentCol) = AlgebraicToIndices(currentPosition);
            var (newRow, newCol) = AlgebraicToIndices(newPosition);

            return currentRow == newRow || currentCol == newCol;
        }
    }
}