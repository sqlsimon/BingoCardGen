using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.IO;
using NGraphics;
using System.Globalization;

//using System.Drawing;


namespace BingoCardGen
{

    class BingoCardItem
    {
        private string imagepath;
        private string label;


        public BingoCardItem()
        {
            imagepath = null;
            label = "empty";
        }

        public string ImagePath
        {
            get { return imagepath; }
            set {imagepath = value; }
        }

        public string Label
        {
            get { return label; }
            set { label = value; }
        }


    }


    class BingoCard
    {
        private int rows = 4;
        private int columns = 8;

        private int xpadding = 40;
        private int ypadding= 40;

        private string[,] cardImages;

        private string[,] cardLabels;

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
            cardLabels = new string[rows, columns];

            // sort out images
            for (int c=0;c<=columns-1;c++)
            {
                for (int r=0;r<=rows-1;r++)
                {
                    cardImages[r, c] = @"images\empty.png";
                }
            }

            // sort out labels
            for (int c = 0; c <= columns - 1; c++)
            {
                for (int r = 0; r <= rows - 1; r++)
                {
                    cardLabels[r, c] = "";
                }
            }
        }

        public BingoCard(int cardRows, int cardColumns)
        {
            rows = cardRows;
            columns = cardColumns;

            colWidth = 100;
            rowHeight = 100;

            cardWidth = colWidth * columns;
            cardHeight = rowHeight * rows;

            canvasWidth = cardWidth + 10;
            canvasHeight = cardHeight + 10;

            cardFrameColour = "#0000FF";
            cardFrameWidth = 3;

            cardImages = new string[rows, columns];
            cardLabels = new string[rows, columns];

            // sort out images
            for (int c = 0; c <= columns - 1; c++)
            {
                for (int r = 0; r <= rows - 1; r++)
                {
                    cardImages[r, c] = @"images\empty.png"; ;
                }
            }

            // sort out labels
            for (int c = 0; c <= columns - 1; c++)
            {
                for (int r = 0; r <= rows - 1; r++)
                {
                    cardLabels[r, c] = "";
                }
            }
        }

        public bool LoadCardImage(int row, int column, string filepath,string label = "")
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
                    cardLabels[row, column] = label;
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
            var b = new SolidBrush(Colors.Black);
            var f = new Font
            {
                Size = 20,
            };

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
            int imageypos = (ypadding / 2) - 10; // the -10 is to nugde the image towards the top of box so we can fit text in

            //int xoffset = 0 + xpadding / 2;
            //int yoffset = 0 + ypadding / 2;

            //var img = Platforms.Current.LoadImage(@"images\presents\blue-present.png");
            // draw the images

            var img = Platforms.Current.LoadImage(@"images\empty.png");

            for (int n=0;n<= rows-1;n++) 
            {
                imageypos = n * rowHeight + ((ypadding / 2) - 10);
                

                for (int m=0;m<=columns-1;m++)
                {
                   
                    imagexpos = m * colWidth + (xpadding / 2);
                    var imagePath = cardImages[n, m];
                    if (File.Exists(imagePath))
                        img = Platforms.Current.LoadImage(imagePath);


                    //System.Console.WriteLine("Height: {0} Width: {1}", img.Size.Height, img.Size.Width);
                    canvas.DrawImage(img, imagexpos, imageypos , imagewidth, imageHeight);
                    Point pt = new Point(imagexpos, imageypos + imageHeight + 25);
                    canvas.DrawText(cardLabels[n,m], pt, f, b);

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
                    System.Console.Write("{0} [{1}]|",cardImages[r, c],cardLabels[r,c]); 
                }
                System.Console.WriteLine("");
            }
        }

    }
    
    
    
    
    
    class BingoCardGen
    {

        static ArrayList LoadImagesFromDisk()
        {
            ArrayList carditems = new ArrayList();
            string[] files = Directory.GetFiles("images", "*.png", SearchOption.AllDirectories);

            // load in images files 
            for (int n = 0; n <= files.Length - 1; n++)
            {
                if (!files[n].Contains("empty.png")) 
                { 
                    //System.Console.WriteLine(files[n]);
                    BingoCardItem bci = new BingoCardItem();
                    bci.ImagePath = files[n];
                    bci.Label = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(files[n].Split('\\')[2].Split('-')[0]);
                    carditems.Add(bci);
                }
            }

            return carditems;
        }

        static ArrayList CreateRandomCards(int numberOfCards, int numberOfSlots, int blankSlots = 4, int randMax = 38)
        {
            // Generate a bunch of arrays of random numbers
            Random rn = new Random();
            Random bn = new Random();

            // an array of arrays (each array is a card)
            ArrayList cards = new ArrayList();

            //create 10 cards
            for (int noCards = 0; noCards <= numberOfCards; noCards++)
            {
                //card
                ArrayList card = new ArrayList();

                // generate 16 numbers (for a card with 16 slots)
                for (int i = 0; i <= numberOfSlots - 1; i++)
                {

                    int rInt = rn.Next(0, randMax-1); // we have 38 images so generate a random number between 0 and 37

                    while (card.Contains(rInt))
                        rInt = rn.Next(0, randMax - 1);

                    card.Add(rInt);

                }
                //System.Console.WriteLine(rInt);

                //blank out some of the slots 
                for (int i = 0; i <= blankSlots; i++)
                {

                    int bInt = bn.Next(0, numberOfSlots - 1); 
                    
                    while (card.Contains(bInt))
                        bInt = bn.Next(0, numberOfSlots - 1);

                    card[bInt] = -1;

                    //System.Console.WriteLine(i);

                }

                cards.Add(card);

            }

            return cards;

        }
        static void Main(string[] args)
        {

            ArrayList itemImages = LoadImagesFromDisk();


            ArrayList cards = CreateRandomCards(5, 16, 4, 38);

            // Need to check that we have no cards the same
            // Put some blank spaces in the cards [partially complete]
            // need to take the cards arrays and turn them into picture bingo cards

            BingoCard b = new BingoCard(4,4);

            // loop over each array of numbers generated by CreateRandomCards
            // create a bingo card and then loop over the rows and cols and 
            // load the images 
            //
            // 
            for (int crds = 0; crds <= cards.Count-1; crds++)
            {
                BingoCard b1 = new BingoCard(4, 4);

                ArrayList currCard = (ArrayList)cards[crds]; // get the array of numbers we use to pick images

                int counter = 0;
                BingoCardItem crd;

                for (int r = 0; r <= 3; r++)
                {
                    for (int c = 0; c <= 3; c++)
                    {
 
                        System.Console.WriteLine("{0} {1} {2}",counter,r,c);
                        int mystuff = (int)currCard[counter];
                        if (mystuff != -1) 
                        { 
                            crd = (BingoCardItem)itemImages[mystuff];
                            b1.LoadCardImage(r, c, crd.ImagePath, crd.Label);
                         }   
                        else
                        {
                            b1.LoadCardImage(r, c, @"images\empty.png", "");
                        }
                        counter++;


                    }
                }
                b1.drawCard();
                IImageCanvas bcc = b1.drawCard();
                bcc.GetImage().SaveAsPng("object_" + crds.ToString() + ".png");

            }

            
            System.Console.Read();


        }
    }
}
