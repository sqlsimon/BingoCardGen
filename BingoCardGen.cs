using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NGraphics;
//using System.Drawing;


namespace BingoCardGen
{

    class BingoCard
    {
        private int rows = 4;
        private int columns = 8;

        private int xpadding = 40;
        private int ypadding= 40;

        private string[,] cardImages;

        /// ///////////////////////////////////////////////////////////////////////


         private int colWidth;
         private int rowHeight;

        private int cardWidth;
        private int cardHeight;
        private string cardFrameColour;
        private int cardFrameWidth;

        private int canvasWidth;
        private int canvasHeight;

        /// ///////////////////////////////////////////////////////////////////////
        
        public BingoCard()
        {
            rows = 4;
            columns = 8;

            colWidth = 100;
            rowHeight = 100;

            cardWidth = colWidth * columns;
            cardHeight = rowHeight * rows;

            canvasWidth = cardWidth + 10;
            canvasHeight = cardHeight + 10;

            cardFrameColour = "#0000FF";
            cardFrameWidth = 3;

            cardImages = new string[rows, columns];

            for(int c=0;c<=columns-1;c++)
            {
                for (int r=0;r<=rows-1;r++)
                {
                    cardImages[r, c] = "Placeholder.png";
                }
            }
        }

        public bool LoadCardImage(int row, int column, string filepath)
        {
            bool found = false;

            if (row >= 0 && row <= this.rows - 1 &&
                column >= 0 && column <= columns - 1)
            {

                // check that the card doesnt already have this image
                for (int c = 0; c <= columns - 1; c++)
                {
                    for (int r = 0; r <= rows - 1; r++)
                    {
                        if (cardImages[r, c] == filepath)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                // if we dont find the image in the card then we can add it.
                if (!found) { 
                    cardImages[row, column] = filepath;
                }
                return (!found);
            }
            else
            {
                // if array coords out of bounds then return false;
                return false;
            }
                
        }

        public IImageCanvas drawCard()
        {
            var size = new Size(canvasWidth, canvasHeight);
            var canvas = Platforms.Current.CreateImageCanvas(size);
            canvas.SaveState();


            var r = new Rect(1, 1, cardWidth, cardHeight);
            var c = new Color(cardFrameColour);

            canvas.DrawRectangle(r, c, cardFrameWidth);

            var colrect = new Rect(1, 1, colWidth, cardHeight);


            // draw the columns
            for (int n = 1; n <= canvasWidth; n = n + colWidth)
            {
                var p1 = new Point(n, 1);
                var p2 = new Point(n, cardHeight);
                canvas.DrawLine(p1, p2, c, cardFrameWidth);
            }

            // draw the rows
            for (int m = 1; m <= canvasHeight; m = m + rowHeight)
            {
                var p1 = new Point(1, m);
                var p2 = new Point(cardWidth, m);
                canvas.DrawLine(p1, p2, c, cardFrameWidth);
            }


            int imagewidth = colWidth - xpadding;
            int imageHeight = rowHeight - ypadding;

            int imagexpos = xpadding / 2;
            int imageypos = ypadding / 2;

            //int xoffset = 0 + xpadding / 2;
            //int yoffset = 0 + ypadding / 2;

            var img = Platforms.Current.LoadImage(@"C:\Users\sime\Downloads\christmas-present.png");
            // draw the images
            for (int n=0;n<= rows-1;n++) 
            {
                imageypos = n * rowHeight + (ypadding / 2);

                for (int m=0;m<=columns-1;m++)
                {

                    imagexpos = m * colWidth + (xpadding / 2);
                    //var imagePath = cardImages[n, m];
                    //var img = Platforms.Current.LoadImage(imagePath);


                    System.Console.WriteLine("Height: {0} Width: {1}", img.Size.Height, img.Size.Width);
                    canvas.DrawImage(img, imagexpos, imageypos , imagewidth, imagewidth);

                }
            }






            return canvas;
        }

        public void Dump()
        {
            for (int r = 0; r <= rows-1; r++)
            {
                System.Console.Write("|");
                for (int c = 0; c <= columns-1; c++)
                {
                    System.Console.Write("{0}|",cardImages[r, c]); 
                }
                System.Console.WriteLine("");
            }
        }

    }
    class BingoCardGen
    {
        static void Main(string[] args)
        {


            

            BingoCard b = new BingoCard();
            //b.Dump();

            System.Console.WriteLine(b.LoadCardImage(10, 3,"sausage.png"));
            System.Console.WriteLine(b.LoadCardImage(2, 4,"beans.png"));
            System.Console.WriteLine(b.LoadCardImage(3,2, "sausage.png"));

            b.Dump();
            b.drawCard();
            IImageCanvas bc = b.drawCard();
            bc.GetImage().SaveAsPng("object.png");
                
            
            System.Console.ReadLine();


        }
    }
}
