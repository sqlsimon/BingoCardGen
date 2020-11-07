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
using System.Data;
using Fclp;  // fluent command line parser
using Serilog;
using Serilog.Sinks.SystemConsole;
using System.Collections.Specialized;

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

            cardFrameColour = "#FF0000";
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

            cardFrameColour = "#FF0000";
            cardFrameWidth = 3;

            cardImages = new string[rows, columns];
            cardLabels = new string[rows, columns];

            // sort out images
            for (int c = 0; c <= columns - 1; c++)
            {
                for (int r = 0; r <= rows - 1; r++)
                {
                    cardImages[r, c] = @"images\card-items\empty.png"; ;
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

            var img = Platforms.Current.LoadImage(@"images\card-items\empty.png");

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
    
    
    public class ApplicationArguments
    {
        public int Rows { get; set; }        // number of rows on each card
        public int Columns { get; set; }    // number of columns on each card
        public int EmptySlots { get; set; }  // empty boxes per card
        public int Cards { get; set; }       // number of cards to generate

        public string OutputPath { get; set; }       //path to output card images to 
        public string ImageRootPath { get; set; }    //root path to images to use on bingo cards
    }
    
    
    
    class BingoCardGen
    {

        static ArrayList LoadImagesFromDisk(string imageSourcePath)
        {
            ArrayList carditems = new ArrayList();
            string[] files = Directory.GetFiles(imageSourcePath, "*.png", SearchOption.AllDirectories);

            // load in images files 
            for (int n = 0; n <= files.Length - 1; n++)
            {
                if (!files[n].Contains("empty.png")) 
                { 
                    //System.Console.WriteLine(files[n]);
                    BingoCardItem bci = new BingoCardItem();
                    bci.ImagePath = files[n];
                    bci.Label = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(files[n].Split('\\')[3].Split('-')[0]);
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
        
        static bool ValidateCards(ArrayList cards)
        {
            for (int a = 0; a <= cards.Count-1; a++)
            {
                
                for (int b = a+1; b <= cards.Count - 1; b++)
                {
                    ArrayList left = (ArrayList)cards[a];
                    ArrayList right = (ArrayList)cards[b];

                    ArrayList left_c = (ArrayList)left.Clone();
                    ArrayList right_c = (ArrayList)right.Clone();

                    left_c.Sort();
                    right_c.Sort();

                    Log.Information("Comparing Cards: {0}, {1},", a, b);

                    if (AreEqual(left_c, right_c))
                    {
                        Log.Information("Cards are the same");
                        Log.Information("Left: [{0}]",DumpCard(left_c));
                        Log.Information("Rght: [{0}]", DumpCard(right_c));

                        return false;
                    }
                    else
                    {
                        Log.Information("Cards are different");
                        Log.Information("Left: [{0}]", DumpCard(left_c));
                        Log.Information("Right: [{0}]", DumpCard(right_c));
                    }
                        
                }
            }
            return true;
        }

        static void CheckForEmptySlots(ArrayList cards, int EmptySlotsReqired, int cardsize, int randMax = 38 )
        {

            Random rn = new Random();
            Random bn = new Random();
           

            // loop over each card 
            for (int a = 0; a <= cards.Count - 1; a++)
            {
                int EmptySlotCount = 0;
                ArrayList current = (ArrayList)cards[a];

                for (int b = 0; b <= current.Count - 1; b++)
                {

                    if ((int)current[b] == -1)
                    {
                        EmptySlotCount++;
                    }
                }

                if (EmptySlotCount != EmptySlotsReqired)
                {
                    Log.Warning("Card {0} has {1} empty slots", a, EmptySlotCount);
                    int SlotsToAdd = EmptySlotsReqired - EmptySlotCount;

                    if (SlotsToAdd <0)
                    {
                    // a negative slotstoadd means that we have to remove some
                    // empty cells and repace them
                        while( SlotsToAdd < 0) 
                        {

                            //generate a random number
                            int rInt = rn.Next(0, randMax - 1);

                            while (current.Contains(rInt))
                                rInt = rn.Next(0, randMax - 1);

                            //find an empty cell to replace
                            for (int d = 0; d <= cardsize - 1; d++)
                            {
                                if ((int)current[d] == -1)
                                { 
                                    current[d] = rInt;
                                    break;
                                }

                            }

                            SlotsToAdd++;

                        }

                    }
                    else
                    {
                    // a positve slotstoadd means that we have to add some blank
                    // cells.
                        while (SlotsToAdd > 0)
                        {
                          
                            int bInt = bn.Next(0, cardsize - 1);


                            if ((int)current[bInt] != -1)
                            {
                                current[bInt] = -1;
                                SlotsToAdd--;
                            }
                            
                        }

                    }

                }
                else
                {
                    Log.Information("Card {0} Has {1} empty slots",a,EmptySlotCount);
                }
            }

 
        }

        static bool AreEqual(ArrayList a, ArrayList b)
        {
            if (a.Count == b.Count)
            {
                for (int i = 0; i <= a.Count - 1; i++)
                {
                    if ((int)a[i] != (int)b[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                System.Console.WriteLine("Arrays are of different sizes");
                return false;
            }
        }

        static string DumpCard(ArrayList a)
        {
            StringBuilder s = new StringBuilder();

            for (int i=0;i <= a.Count-1; i++)
            {
                s.Append(a[i].ToString());
                if (i < a.Count - 1) s.Append(",");
            }
            return s.ToString();
        }


        private static void BuildCardImages(int rows, int columns, ArrayList itemImages, ArrayList cards)
        {
            // loop over each array of numbers generated by CreateRandomCards
            // create a bingo card and then loop over the rows and cols and 
            // load the images 
            //
            for (int card = 0; card <= cards.Count - 1; card++)
            {
                BingoCard bingocard = new BingoCard(rows, columns);

                ArrayList currentcard = (ArrayList)cards[card]; // get the array of numbers we use to pick images

                int counter = 0;
                BingoCardItem carditem;

                for (int r = 0; r <= rows - 1; r++)
                {
                    for (int c = 0; c <= columns - 1; c++)
                    {

                        System.Console.WriteLine("Slot:{0} Row:{1} Column:{2}", counter, r, c);

                        // if the current number is a -1 this means render a blank space
                        // otherwise show the image from the itemImages array
                        if ((int)currentcard[counter] != -1)
                        {
                            carditem = (BingoCardItem)itemImages[(int)currentcard[counter]];
                            bingocard.LoadCardImage(r, c, carditem.ImagePath, carditem.Label);
                        }
                        else
                        {
                            bingocard.LoadCardImage(r, c, @"images\card-items\empty.png", "");
                        }
                        counter++;


                    }
                }
                bingocard.drawCard();
                IImageCanvas bcc = bingocard.drawCard();
                bcc.GetImage().SaveAsPng("object_" + card.ToString() + ".png");

            }
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            var p = new FluentCommandLineParser<ApplicationArguments>();

            p.Setup(arg => arg.Rows)
             .As('r', "rows")
             .SetDefault(8); 

            p.Setup(arg => arg.Columns)
             .As('c', "columns")
             .SetDefault(4);

            p.Setup(arg => arg.EmptySlots)
             .As('e', "emptyslots")
              .SetDefault(4);

            p.Setup(arg => arg.Cards)
             .As('n', "number-of-cards")
             .Required();

            p.Setup(arg => arg.ImageRootPath)
             .As('i', "image-root-path")
             .SetDefault(@"images\card-items");

            p.Setup(arg => arg.OutputPath)
             .As('o', "output-path")
             .SetDefault("output");

            var result = p.Parse(args);

            Log.Information("Parsing command line args");

            if (!result.HasErrors)
            {

                Log.Information("Command line args OK");

                Log.Information("Reading images from: {0}", p.Object.ImageRootPath);


                // Load the images from disk into an Array of BingoCardItems
                // A BingoCardItem has a path to an image and a label. The 
                // image filename has the assumed format "<colourname>-<somestring>,png"
                // The Load images function splits the filename and uses the colourname
                // part as the label.
                ArrayList itemImages = LoadImagesFromDisk(p.Object.ImageRootPath);


                Log.Information("Creating {0} random number arrays with {1} rows and {2} columns.", p.Object.Cards, p.Object.Rows, p.Object.Columns);
                // The createRandomCards function returns an array of arrays. 
                // Each array represents a bingo card and 16 random numbers 
                // between 0 and a max value (the max value being the last parameter 
                // of the function)
                ArrayList cards = CreateRandomCards(p.Object.Cards, (p.Object.Rows * p.Object.Columns), p.Object.EmptySlots, itemImages.Count);

                // TODO Need to check that we have no cards the same
                // TODO Put some blank spaces in the cards [partially complete]
                // TODO Check that we have the required number of spaces on each card

                Log.Information("Validating random number arrays"); 
                bool CardsValidated = ValidateCards(cards);
                
                // Check for incorrect number of empty slots and correct
                CheckForEmptySlots(cards, p.Object.EmptySlots, 16,38);
                System.Console.WriteLine("===========================================");
                
                // Peform check for empty slots, we should now have correct
                // number of empty slots on each card
                CheckForEmptySlots(cards, p.Object.EmptySlots, 16, 38);

                // If the cards are validated proceed to creating the graphical 
                // representations of the cards with the images
                if (CardsValidated)
                {
                    Log.Information("Bingo cards successfully validated ");
                    // itemimages is an array of BingoCardItem objects (pictures / labels)
                    // cards is an array of arrays of ints representing the bingo cards


                    Log.Information("Generatign Bingo cards");
                    BuildCardImages(p.Object.Rows, p.Object.Columns, itemImages, cards);
                }

                System.Console.Read();

            }
            else
            {
                Log.Error("Error processing command line options.");
            }
        }

        
    }
}
