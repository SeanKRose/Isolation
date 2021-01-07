using System;
using static System.Console;

namespace Bme121
{
    static class Program
    {
        static bool[ , ] tileExists; //True if the tile exists, false if tile is removed.
        static string playerAName, playerBName;
        static string pa;  //Player A's pawn symbol
        static string pb;  //Player B's pawn symbol
        //The following 8 variables are initialized to - 1 because it is guarenteed to be outside 
        //of the board display range during the set up phase.
        static int playerAPlatformRow = - 1;
        static int playerAPlatformColumn = - 1;
        static int playerBPlatformRow = - 1;
        static int playerBPlatformColumn = - 1;
        static int currentRowOfA = - 1;
        static int currentColumnOfA = - 1;
        static int currentRowOfB = - 1;
        static int currentColumnOfB = - 1;   
        static bool gameIsOver = false;
        static bool playerATurn = true; 
        static string[ ] letters = { "a","b","c","d","e","f","g","h","i","j","k","l",
                            "m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
        
        static void Setup( )
        {
            //Ask and set player names and symbols.
            WriteLine( "Player A, what is your name? (The default is Player A)" );
            playerAName = ReadLine ( );
            if( playerAName.Length == 0 ) playerAName = "Player A";
            pa = playerAName.Substring( 0, 1 );

            WriteLine( "Player B, what is your name? (The default is Player B)" );
            playerBName = ReadLine ( );
            if( playerBName.Length == 0 ) playerBName = "Player B";
            pb = playerBName.Substring( 0, 1 );

            if( pa == pb )  // Set the default symbols (in case both symbols would be the same).
            {
                pa = "A";
                pb = "B";
            }
            
            WriteLine( "Player A, your name is " + playerAName + " and your pawn will be" 
                        + "represented by the symbol " + pa );
            WriteLine( "Player B, your name is " + playerBName + " and your pawn will be" 
                        + "represented by the symbol " + pb );
            
            //Assign dimensions for the game board
            bool invalidDimensions = true;
            int boardWidth = 6;
            int boardLength = 8;
            while( invalidDimensions )
            {
                invalidDimensions = false;
                WriteLine( "The dimensions of the game board must be greater than 3 and less than 27." );
                WriteLine( "What width would you like your game board to be? (The default is 6)" );
                string response = ReadLine( );
                if( response.Length == 0 ) boardWidth = 6;
                else boardWidth = int.Parse( response );
                if( boardWidth < 4 || boardWidth > 26 ) invalidDimensions = true;
                else 
                {
                    WriteLine( "What length would you like your game board to be? (The default is 8)" );
                    response = ReadLine( );
                    if( response.Length == 0 ) boardLength = 8;
                    else boardLength = int.Parse( response );
                    if( boardLength < 4 || boardLength > 26 ) invalidDimensions = true;
                }
            }
            
            //Set all spaces on the board to true
            tileExists = new bool[boardLength, boardWidth];
            for( int i = 0; i < tileExists.GetLength(0); i ++ )
                {
                    for( int j = 0; j< tileExists.GetLength(1); j ++ )
                    {
                        tileExists[ i, j ] = true;
                    }
                }
                
            DisplayBoard();
            
            //Prompt the user for the platform location of Player A. Set the platform location.
            bool invalidPlatformLocation = true;
            while( invalidPlatformLocation )
            {
                invalidPlatformLocation = false;
                WriteLine( playerAName + ", enter the location of your starting platform (ex: ac): " );
                string playerAPlatform = ReadLine ( );
                if( playerAPlatform.Length != 2 )
                {
                    currentRowOfA = playerAPlatformRow = 0;
                    currentColumnOfA = playerAPlatformColumn = ( int ) Math.Ceiling( (boardWidth - 1) / 2d );
                }    
                else 
                {
                    playerAPlatformRow = Array.IndexOf( letters, playerAPlatform.Substring( 0, 1 ) );
                    playerAPlatformColumn = Array.IndexOf( letters, playerAPlatform.Substring( 1, 1 ) );
                    
                    if(    playerAPlatformRow    < 0
                        || playerAPlatformRow    > boardLength - 1
                        || playerAPlatformColumn < 0
                        || playerAPlatformColumn > boardWidth - 1 )
                    {
                        WriteLine( "That platform location is invalid. Please enter a valid input." );
                        invalidPlatformLocation = true;
                    }
                    else
                    {
                        currentRowOfA = playerAPlatformRow;
                        currentColumnOfA = playerAPlatformColumn;
                    }
                }
            }
            
            //Prompt the user for the platform location of Player B. Set the platform location.
            invalidPlatformLocation = true;
            while( invalidPlatformLocation )
            {
                invalidPlatformLocation = false;
                WriteLine( playerBName + ", enter the location of your starting platform (ex: ac): " );
                string playerBPlatform = ReadLine ( );
                if( playerBPlatform.Length != 2 )
                {
                    currentRowOfB = playerBPlatformRow = boardLength - 1;
                    currentColumnOfB = playerBPlatformColumn = ( int ) Math.Floor( (boardWidth - 1) / 2d );
                }    
                else 
                {
                    playerBPlatformRow = Array.IndexOf( letters, playerBPlatform.Substring( 0, 1 ) );
                    playerBPlatformColumn = Array.IndexOf( letters, playerBPlatform.Substring( 1, 1 ) );

                    if(      playerBPlatformRow    < 0
                        ||   playerBPlatformRow    > boardLength - 1
                        ||   playerBPlatformColumn < 0
                        ||   playerBPlatformColumn > boardWidth - 1 
                        || ( playerBPlatformRow == playerAPlatformRow && playerBPlatformColumn == playerAPlatformColumn ) )
                    {
                        WriteLine( "That platform location is invalid. Please enter a valid input." );
                        invalidPlatformLocation = true;
                    }
                    else
                    {
                        currentRowOfB = playerBPlatformRow;
                        currentColumnOfB = playerBPlatformColumn;
                    }
                }
            }
            return;
        }
        
        static void DisplayBoard()
        {
            const string h  = "\u2500"; // horizontal line
            const string v  = "\u2502"; // vertical line
            const string tl = "\u250c"; // top left corner
            const string tr = "\u2510"; // top right corner
            const string bl = "\u2514"; // bottom left corner
            const string br = "\u2518"; // bottom right corner
            const string vr = "\u251c"; // vertical join from right
            const string vl = "\u2524"; // vertical join from left
            const string hb = "\u252c"; // horizontal join from below
            const string ha = "\u2534"; // horizontal join from above
            const string hv = "\u253c"; // horizontal vertical cross
            string sp = " ";            // space
            string bb = "\u25a0";       // small block
            string fb = "\u2588";       // full block
            string lh = "\u258c";       // left half block
            string rh = "\u2590";       // right half block
                                  
            //Draw the top boundary
            Write( "   " );
            for( int r = 0; r < tileExists.GetLength( 1 ); r ++ )
            {
                Write( " {0}  ", letters[ r ] );
            }
            WriteLine( "      " );
            Write( "  ");
            for( int c = 0; c < tileExists.GetLength(1); c ++)
            {
                if( c == 0 ) Write(tl);
                Write( "{0}{0}{0}", h);
                if(c == tileExists.GetLength(1) - 1 ) Write( "{0}", tr);
                else Write( "{0}", hb);
            }
            WriteLine();
            
            // Draw the board rows
            for( int r = 0; r < tileExists.GetLength( 0 ); r ++ )
            {
                Write( "{0} ", letters[ r ] );
                
                // Draw the row contents
                for( int c = 0; c <= tileExists.GetLength( 1 ); c ++)
                {
                    if( c == 0 ) Write(v);
                    else if( r == currentRowOfA && c == currentColumnOfA + 1 )  
                            Write( $"{sp}{pa}{sp}{v}" );
                    else if( r == currentRowOfB && c == currentColumnOfB + 1)  
                            Write( $"{sp}{pb}{sp}{v}" );
                    else if( r == playerAPlatformRow  & c == playerAPlatformColumn + 1) 
                            Write( $"{sp}{bb}{sp}{v}" );
                    else if( r == playerBPlatformRow & c == playerBPlatformColumn + 1) 
                            Write( $"{sp}{bb}{sp}{v}" );
                    else if( tileExists[ r, c - 1 ] == true )                
                            Write( $"{rh}{fb}{lh}{v}" );
                    else    Write( $"{sp}{sp}{sp}{v}" );
                }
                WriteLine( );
                
                // Draw the boundary after the row.
                if( r != tileExists.GetLength(0) - 1 )
                {
                    Write( "  ");
                    for( int c = 0; c < tileExists.GetLength(1); c ++)
                    {
                        if( c == 0 ) Write(vr);
                        Write( "{0}{0}{0}", h);
                        if(c == tileExists.GetLength(1) - 1 ) Write( "{0}", vl);
                        else Write( "{0}", hv);
                    }
                    WriteLine();
                }
                else 
                {
                    Write( "  ");
                    for( int c = 0; c < tileExists.GetLength(1); c ++)
                    {  
                    if( c == 0 ) Write(bl);
                    Write( "{0}{0}{0}", h);
                    if(c == tileExists.GetLength(1) - 1 ) Write( "{0}", br);
                    else Write( "{0}", ha);
                    }
                    WriteLine ( );
                }
            }
        }
        
        static void PlayerMoveInput()
        {
            int nextRow = 0;
            int nextColumn = 0;
            int removeRowNumber = 0;
            int removeColumnNumber = 0;
            string moveInput = "0";
            bool invalidMove = true;
            bool invalidInput = true;
            
            while (invalidMove) 
            {
                while(invalidInput)
                {
                    //Prompt the input from the user and check if they would like to end the game.
                    if( playerATurn )
                        WriteLine( playerAName + ", enter your next move (for ex. abcd) "
                        + "or end the game by typing 'End Game'." );
                    if( ! playerATurn )
                        WriteLine( playerBName + ", enter your next move (for ex. abcd) "
                        + "or end the game by typing 'End Game'." );
                    
                    moveInput = ReadLine();
                    if( moveInput == "End Game" ) 
                    {
                        gameIsOver = true;
                        return;
                    }
                    else if( moveInput.Length == 4 ) invalidInput = false;
                    else WriteLine( "That input was invalid! Ensure you are inputting 4 letters." );
                }
                
                nextRow            = Array.IndexOf( letters, moveInput.Substring( 0, 1 ) );
                nextColumn         = Array.IndexOf( letters, moveInput.Substring( 1, 1 ) );
                removeRowNumber    = Array.IndexOf( letters, moveInput.Substring( 2, 1 ) );
                removeColumnNumber = Array.IndexOf( letters, moveInput.Substring( 3, 1 ) );
                
                //Check if the new pawn location is invalid
                if(    nextRow    < 0 || nextRow    > tileExists.GetLength(0) - 1          
                    || nextColumn < 0 || nextColumn > tileExists.GetLength(1) - 1
                    || tileExists[ nextRow, nextColumn ] == false             
                    || ( nextRow == currentRowOfA && nextColumn == currentColumnOfA ) 
                    || ( nextRow == currentRowOfB && nextColumn == currentColumnOfB ) 
                    || (   playerATurn && ( int ) Math.Abs( currentRowOfA - nextRow )       > 1 )
                    || (   playerATurn && ( int ) Math.Abs( currentColumnOfA - nextColumn ) > 1 )
                    || ( ! playerATurn && ( int ) Math.Abs( currentRowOfB - nextRow )       > 1 )
                    || ( ! playerATurn && ( int ) Math.Abs( currentColumnOfB - nextColumn ) > 1 ) )
                {
                    WriteLine( "The new pawn location you have entered is invalid. " 
                               + "Enter a valid location or press 'Ctrl' 'C' to end the program." );
                    invalidInput = true;
                }
                
                //Check if the removed tile location is invalid
                else if(   removeRowNumber    < 0 || removeRowNumber    > tileExists.GetLength(0) - 1
                        || removeColumnNumber < 0 || removeColumnNumber > tileExists.GetLength(1) - 1
                        || tileExists[ removeRowNumber, removeColumnNumber ] == false  
                        || (   playerATurn && removeRowNumber == currentRowOfB && removeColumnNumber == currentColumnOfB )
                        || ( ! playerATurn && removeRowNumber == currentRowOfA && removeColumnNumber == currentColumnOfA )
                        || (   removeRowNumber == playerAPlatformRow && removeColumnNumber == playerAPlatformColumn )
                        || (   removeRowNumber == playerBPlatformRow && removeColumnNumber == playerBPlatformColumn ) 
                        || (   removeRowNumber == nextRow            && removeColumnNumber == nextColumn ) )
                {
                    WriteLine( "The tile you are trying to remove is invalid."
                            + "Enter a valid tile or press 'Ctrl' 'C' to end the program." );
                    invalidInput = true;
                }
                else invalidMove = false;
            }
            
            //If the move is valid, the removed tile becomes removed on the board. 
            //Then, the new location of the turn player is updated.
            tileExists[ removeRowNumber, removeColumnNumber ] = false;
            if( playerATurn )
            {
                currentRowOfA = nextRow;
                currentColumnOfA = nextColumn;
            }
            
            if( ! playerATurn )
            {
                currentRowOfB = nextRow;
                currentColumnOfB = nextColumn;
            }
            
            playerATurn = ! playerATurn;
        }
        
        static void Main( )
        {
            Setup( );
            Console.Clear( );
            
            while( ! gameIsOver )
            {
                DisplayBoard( );
                PlayerMoveInput( );
                Console.Clear( ); 
            }
            
            DisplayBoard( );
            WriteLine( "Who won?" );
            WriteLine( "Congratulations! " + ReadLine( ) + " is the winner!" );
        }
    }
}
