package es.ucm.gdv.ohno;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Stack;

import es.ucm.gdv.engine.AbstractScene;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;
import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

public class OhnO_Game extends AbstractScene {
    private final float _aspectRatio = 2f/3f;

    private Graphics _graphics;
    private Input _input;
    private float _fontSize;

    // Assets
    private Font _fontMolle, _fontJose;
    private Image _closeImg, _historyImg, _eyeImg, _lockImg;


    private int _numCircles;
    private boolean _win = false;//si se ha ganado

    private Square[][] board;     // Tablero con todas las casillas
    private float _yBoardOffset;
    private float _xBoardOffset;
    private float _boardCircleDiam;
    private float _extraCircle;

    private List<Square> locked; // Las casillas lockeadas
    //private Square _hintedSquare;
    //private Square _hintedSquareCircle;//para poder hacer el fadeOut de la pista
    private boolean _undoOnce; //para saber si se ha pulsado deshacer movimiento por primera vez
    private Square _drawBlackSquare;

    private int _numGreys, _startNumGrey;       //numero de casillas grises, para hacer el porcentaje
    private boolean _showLock;   //booleano para mostrar o no el candado en los rojos

    private float _widthImages, _heightImages;

    // Tiempo que tardan los textos en hacer el fade
    private float _textFadeTime = 0.5f;

    private String _sizeText;
    private float _sizeTextAlpha = 0.0f;
    private int _sizeTextFadeFactor = 1;    // 1 = fadeIn // -1 = fadeOut

    private String _hintUndoText;   //texto para las pistas o deshacer movimiento
    private float _hintUndoTextAlpha = 0.0f;
    private int _hintUndoTextFadeFactor = -1;    // 1 = fadeIn // -1 = fadeOut

    //para hacer fade-in al empezar la escena
    private float _sceneFadeTime = 0.5f;
    private float _sceneAlpha = 255.0f;
    private int _sceneFadeFactor = -1;    // 1 = fadeIn // -1 = fadeOut

    //para hacer fade-out al ganar
    private float _sceneOutFadeTime = 3.5f;
    private float _sceneOutAlpha = 255.0f;
    private int _sceneOutFadeFactor = -1;    // 1 = fadeIn // -1 = fadeOut

    private Stack<TouchEvent> _oppositeMoves = new Stack<TouchEvent>();

    public OhnO_Game(int num){
        _numCircles = num;
        initGame();
        _showLock = false;
    }
    @Override
    public void start() {
        _fontSize= 120;
        _graphics = _engine.getGraphics();
        _input = _engine.getInput();
        _fontMolle = _graphics.newFont("assets/fonts/Molle-Regular.ttf", _fontSize, false);
        _fontJose = _graphics.newFont("assets/fonts/JosefinSans-Bold.ttf", _fontSize, false);
        _closeImg = _graphics.newImage("assets/images/close.png");
        _historyImg = _graphics.newImage("assets/images/history.png");
        _eyeImg = _graphics.newImage("assets/images/eye.png");
        _lockImg = _graphics.newImage("assets/images/lock.png");

        _sizeText = _numCircles + " x " + _numCircles;
        _hintUndoText = "";
    }

    @Override
    public void handleInput() {
        List<TouchEvent> events = _input.getTouchEvents();
        while(events.size() > 0) {
            TouchEvent touchEvent = events.get(0);
            events.remove(0);
            processInput(touchEvent);
        }
    }

    private void processInput(TouchEvent e) {
        if (e.getType() != TouchType.Press) return;

        float X = e.getX();
        float Y = e.getY();

        if(Y >= _yBoardOffset && X >= _xBoardOffset) {
            //deteccion input en tablero
            float boardY = Y - _yBoardOffset; // La Y "en el tablero"
            float boardX = X - _xBoardOffset; // La X "en el tablero"
            int indexY = (int) (boardY / ((_boardCircleDiam * _aspectRatio) +
                    ((_extraCircle / (_numCircles - 1)) * _boardCircleDiam * _aspectRatio)));
            int indexX = (int) (boardX / ((_boardCircleDiam) +
                    ((_extraCircle / (_numCircles - 1)) * _boardCircleDiam)));
            if (indexX < _numCircles && indexX >= 0 && indexY < _numCircles && indexY >= 0) { // en tablero
                activateCell(indexX, indexY, e.isRightMouse(), true);
                if(_drawBlackSquare != null ) {
                    _drawBlackSquare.drawBlack = false;
                    _drawBlackSquare = null;
                }
                System.out.println("PULSADO EN CASILLA ( " + indexX + ", " + indexY + " )");
                check(true);
            }
        }

        //deteccion botones de abajo
        float yOffset = 5.5f * 1f/6 ;
        float xOffset = 1f/6 ;
        if(Y >= yOffset - _heightImages/2 && Y <= yOffset + _heightImages/2){
            //Volver atras
            if (X >= 2*xOffset - _widthImages / 2  && X <= 2*xOffset + _widthImages / 2){
                _engine.setScene(new OhnO_SelectSize());
            }
            //deshacer movimiento
            else if (X >= 3*xOffset - _widthImages / 2 && X <= 3*xOffset + _widthImages / 2){
                undoMove();

            }
            //pedir pista
            else if (X >= 4*xOffset - _widthImages / 2 && X <= 4*xOffset + _widthImages / 2){
                if(_hintUndoText == ""/*_hintedSquare == null*/) {
                    _hintUndoText = giveHint();
                    //_hintedSquareCircle = _hintedSquare;
                    _sizeTextFadeFactor = -1;
                    _hintUndoTextFadeFactor = 1;

                }
                else{
                    _hintUndoText = "";
                    if(_drawBlackSquare != null ) {
                        _drawBlackSquare.drawBlack = false;
                        _drawBlackSquare = null;
                    }
                    //_hintedSquare = null;
                    _sizeTextFadeFactor = 1;
                    _hintUndoTextFadeFactor = -1;
                }
            }
        }
    }

    @Override
    public void update(float deltaTime) {
        //fade-in al inicio
        if(_sceneAlpha>0) {
            _sceneAlpha += _sceneFadeFactor * deltaTime * (255.0f / _sceneFadeTime);
            _sceneAlpha = clamp(_sceneAlpha, 0.0f, 255.0f);
        }

        if(_win ) {
            _sceneOutAlpha += _sceneOutFadeFactor * deltaTime * (255.0f / _sceneOutFadeTime);
            _sceneOutAlpha = clamp(_sceneOutAlpha, 0.0f, 255.0f);
            if(_sceneOutAlpha <= 0){
                _engine.setScene(new OhnO_SelectSize());
            }


        }

        // Actualizacion del alpha de las casillas
        for (int i = 1; i < board[0].length - 1; ++i) {
            for (int j = 1; j < board[1].length - 1; ++j){
                board[i][j].update(deltaTime);
            }
        }

        // Texto del tamano del tablero
        _sizeTextAlpha += _sizeTextFadeFactor * deltaTime * (255.0f / _textFadeTime);
        _sizeTextAlpha = clamp(_sizeTextAlpha, 0.0f, 255.0f);


        // Texto de las hints
        _hintUndoTextAlpha += _hintUndoTextFadeFactor * deltaTime * (255.0f / _textFadeTime);
        _hintUndoTextAlpha = clamp(_hintUndoTextAlpha, 0.0f, 255.0f);

    }

    private void renderWin(){

        float fontSize= 75;

        // Tamaño del tablero
        Font f = _fontJose;
        f.setBold(true);
        f.setSize(fontSize);
        _graphics.setFont(f);

        // Escribe el texto de ganar
        _graphics.setColor(0,0,0,255);
        _graphics.drawText("Brillante", 1f/2, 0.1f);

        // Dimensiones del tablero
        _xBoardOffset = 0.1f; // 1 - _xStartOffset el final
        _yBoardOffset = 0.2f;

        // Dibujar tablero
        _extraCircle = 0.5f;   // Círculo fantasma extra para el offset
        _boardCircleDiam = (1f - _xBoardOffset * 2) / (_numCircles + _extraCircle);
        f.setSize(fontSize* _boardCircleDiam *5);
        _graphics.setFont(f);
        float yPos = _yBoardOffset + ((_boardCircleDiam * _aspectRatio) / 2);
        float xPos = _xBoardOffset + (_boardCircleDiam / 2);

        for (int i = 0; i < _numCircles; ++i) {
            for (int j = 0; j < _numCircles; ++j) {
                // Renderizar cada casilla
                board[i+1][j+1].render(_graphics, xPos, yPos, _boardCircleDiam / 2);
                xPos += _boardCircleDiam + (_extraCircle / (_numCircles - 1)) * _boardCircleDiam;
            }
            xPos = _xBoardOffset + (_boardCircleDiam / 2);
            yPos += (_boardCircleDiam * _aspectRatio) + ((_extraCircle / (_numCircles - 1)) * _boardCircleDiam * _aspectRatio);

        }//fin tablero

        // Dibujar el porcentaje
        f.setSize(fontSize/1.5f);
        _graphics.setFont(f);
        _graphics.setColor(150,150,150,(int)_sceneOutAlpha);
        float percent = 1 - ( (float) _numGreys / _startNumGrey);
        percent*=100;
        String num = (int)Math.ceil(percent) + "%";
        _graphics.drawText(num, (1f/2),(1/6f * 4.75f));
    }

    @Override
    public void render() {
        if (_win){
            renderWin();
            return;
        }
        float fontSize= 75;


        // Tamaño del tablero
        Font f = _fontJose;
        f.setBold(true);
        f.setSize(fontSize);
        _graphics.setFont(f);

        // Escribe el tamaño del tablero
        _graphics.setColor(0,0,0,(int)_sizeTextAlpha);
        _graphics.drawText(_sizeText, 1f/2, 0.1f);

        f.setSize(fontSize/1.5f);
        _graphics.setFont(f);
        // Escribe la pista
        _graphics.setColor(0,0,0,(int)_hintUndoTextAlpha);
        _graphics.drawText(_hintUndoText, 1f/2, 0.1f);


        // Dimensiones del tablero
        _xBoardOffset = 0.1f; // 1 - _xStartOffset el final
        _yBoardOffset = 0.2f;

        // Dibujar tablero
        _extraCircle = 0.5f;   // Círculo fantasma extra para el offset
        _boardCircleDiam = (1f - _xBoardOffset * 2) / (_numCircles + _extraCircle);
        f.setSize(fontSize* _boardCircleDiam *5);
        _graphics.setFont(f);
        float yPos = _yBoardOffset + ((_boardCircleDiam * _aspectRatio) / 2);
        float xPos = _xBoardOffset + (_boardCircleDiam / 2);
        Square boardSquare;
        for (int i = 0; i < _numCircles; ++i) {
            for (int j = 0; j < _numCircles; ++j) {
                boardSquare = board[i+1][j+1];
//                if(_hintedSquareCircle != null && _hintedSquareCircle == board[i+1][j+1]) {
//
//                    /*if(_drawBlackSquare != null && _drawBlackSquare != boardSquare)
//                        _drawBlackSquare.drawBlack=false;
//                    _drawBlackSquare =  boardSquare;
//                    _drawBlackSquare.drawBlack = true;*/
//                    if(_hintUndoTextAlpha == 0)
//                        _hintedSquareCircle=null;
//
//                    if(_hintUndoTextAlpha == 0)
//                        _hintedSquareCircle=null;
//                }
                // Dibujar la casilla

                boardSquare.render(_graphics, xPos, yPos, _boardCircleDiam / 2);

                // Dibujar el candado en los rojos lockeados
                if(_showLock && boardSquare.lock && boardSquare.currentState == Square.SquareColor.Red){
                    _graphics.drawImage(_lockImg, 0.55f,0.55f, xPos, yPos);
                }

                xPos += _boardCircleDiam + (_extraCircle / (_numCircles - 1)) * _boardCircleDiam;
            }
            xPos = _xBoardOffset + (_boardCircleDiam / 2);
            yPos += (_boardCircleDiam * _aspectRatio) + ((_extraCircle / (_numCircles - 1)) * _boardCircleDiam * _aspectRatio);

        }//fin tablero

        // Dibujar el porcentaje
        f.setSize(fontSize/1.5f);
        _graphics.setFont(f);
        _graphics.setColor(150,150,150,255);
        float percent = 1 - ( (float) _numGreys / _startNumGrey);
        percent*=100;
        String num = (int)Math.ceil(percent) + "%";
        _graphics.drawText(num, (1f/2),(1/6f * 4.75f));


        // Dibujar las imagenes de la interfaz en el  tercio inferior de la pantalla
        float yOffset = 5.5f * 1f/6;
        float xOffset = 1f/6;
        _graphics.drawImage(_closeImg, 1.0f,1.0f, 2*xOffset, yOffset);
        _graphics.drawImage(_historyImg, 1.0f,1.0f,3*xOffset, yOffset);
        _widthImages = _eyeImg.getCanvasWidth();
        _heightImages = _eyeImg.getCanvasHeight();
        _graphics.drawImage(_eyeImg, 1.0f,1.0f,4*xOffset, yOffset);

        // Dibujar el fade out de las casillas pulsadas
        float cx, cy;

        if(_sceneAlpha>0) {
            _graphics.setColor(255, 255, 255, (int) _sceneAlpha);
            _graphics.fillCircle(0.5f, 0.5f, 1);
        }

    }

    //crear tablero inicial
    public void initGame() {
        //rellenamos el tablero con azules y rojos de forma aleatoria
        //minimo un rojo y un azul que no este rodeado por rojos
        do {
            board = new Square[_numCircles + 2][_numCircles + 2];//tamaño +2 para bordear con rojos
            for (int i = 0; i < board[0].length; ++i) {
                for (int j = 0; j < board[1].length; ++j) {
                    board[i][j] = new Square(255f, 0.2f, 0.5f);
                }
            }
            //_numGreys = _numCircles * _numCircles;
            Random rnd = new Random();
            locked = new ArrayList<>();
            for (int i = 0; i < board[0].length; ++i) {
                for (int j = 0; j < board[1].length; ++j) {
                    //bordeamos de rojos
                    if (i == 0 || i == board[0].length - 1 || j == 0 || j == board[1].length - 1) {
                        board[i][j].solutionState = Square.SquareColor.Red;
                        board[i][j].currentState = Square.SquareColor.Red;
                        board[i][j].lock = true;
                    } else {// rellenar con aleatorios rojos y azules
                        if (rnd.nextFloat() < 0.5) {
                            board[i][j].solutionState = Square.SquareColor.Blue;
                            board[i][j].currentState = Square.SquareColor.Grey;
                        } else {
                            board[i][j].solutionState = Square.SquareColor.Red;
                            board[i][j].currentState = Square.SquareColor.Grey;
                        }
                    }
                    board[i][j].posX = i;
                    board[i][j].posY = j;
                }
            }

            //contar elementos adyacentes de la fila y columna
            //el tablero es cuadrado asi que se puede hacer asi
            for (int i = 1; i < board[0].length - 1; ++i) {
                countRow(i, false);//cuenta los adyacentes azules que hay en esa fila
                countCol(i, false);//cuenta los adyacentes azules que hay en esa columna
            }

            for (int i = 1; i < board[0].length - 1; ++i) {
                for (int j = 1; j < board[0].length - 1; ++j) {
                    board[i][j].total = board[i][j].row + board[i][j].column;
                }
            }

            reveal();

            for (int i = 1; i < board[0].length - 1; ++i) {
                countRow(i, true);//cuenta los adyacentes azules que hay en esa fila
                countCol(i, true);//cuenta los adyacentes azules que hay en esa columna
            }

        } while (locked.size() == 0);//tablero con al menos dos azules juntos

        do {//terminar de crear una solucion valida
            if (!doHint()) {//si no se ha completado el nivel y no se pueden aplicar mas pistas
                reStart(false); //añade un rojo y resetea el nivel al principio
            }
        } while (!check(false));


        reStart(true);
        _startNumGrey = _numGreys;
    }

    /**
     * Funcion auxiliar para restringir un valor
     * dentro de unos limites
     *
     * @param value Valor que se quiere restringir
     * @param min Valor minimo al que se limita el valor
     * @param max Valor maximo al que se limita el valor
     * @return El valor dentro de los limites
     */
    private float clamp(float value, float min, float max){
        float ret = value;
        ret = Math.max(min, ret);
        ret = Math.min(ret, max);
        return ret;
    }


    // Comprueba si se ha solucionado
    public boolean check(boolean player){
        if(!player){
            for(int i = 1; i < board[0].length -1; ++i) {
                for (int j = 1; j < board[1].length - 1; ++j) {
                    if(board[i][j].currentState != board[i][j].solutionState){

                        return false;
                    }
                }
                countRow(i,true);
                countCol(i,true);
            }
        }
        else {
            if(_numGreys != 0) return false;
            _hintUndoText = giveHint();
            if (_hintUndoText != "") {
                _sizeTextFadeFactor = -1;
                _hintUndoTextFadeFactor = 1;
                return false;
            }
            _sizeTextFadeFactor = 1;
            _hintUndoTextFadeFactor = -1;

            for (int i = 1; i < board[0].length - 1; ++i) {
                for (int j = 1; j < board[1].length - 1; ++j) {
                    board[i][j].lock = true;
                }
            }
            _win = true;
        }
        return true;
    }

    // Metodo para reiniciar el nivel y dejarlo como al principio
    // si canFinish es false, significa que el nivel no se puede completar
    // y hay que añadir mas casillas visibles rojas al empezar
    public void reStart(Boolean canFinish){

        //añadir un rojo si no es posible completar el nivel
        if(!canFinish) {
            Square sDif = null;
            for(Square s : locked){
                if(s.total != s.playerColumn + s.playerRow) {
                    sDif = s;
                    break;
                }
            }

            Square s = mustBeRed(sDif);
            s.lock=true;
            s.currentState = s.solutionState;
            //System.out.println("Añadido rojo lockeado en "+(s.posX -1) + " "+ (s.posY -1));

        }//rojo añadido
        _numGreys=0;
        //resetear valores
        for(int i = 1; i < board[0].length -1; ++i) {
            for (int j = 1; j < board[1].length - 1; ++j) {
                if(!board[i][j].lock) {
                    board[i][j].currentState = Square.SquareColor.Grey;
                    _numGreys++;
                }
                else if(board[i][j].solutionState == Square.SquareColor.Blue)
                    board[i][j].currentState = Square.SquareColor.Blue;
                else if(board[i][j].solutionState == Square.SquareColor.Red)
                    board[i][j].currentState = Square.SquareColor.Red;

                board[i][j].playerColumn = 0;
                board[i][j].playerRow = 0;
            }
            countRow(i,false);//cuenta los adyacentes azules que hay en esa fila
            countCol(i,false);//cuenta los adyacentes azules que hay en esa columna
        }

    }

    //encontrar un gris adyacente a square y cambiarlo a rojo
    private Square mustBeRed(Square square){
        if (square == null )
            return board[0][0]; //programacion defensiva
        for(Dirs d: Dirs.values()) {//recorrer en todas las direcciones
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            //buscar un gris adyacente sin pasar por rojo
            while(x != 0 && x != board[0].length -1 && y != 0 && y != board[1].length -1 )
            {
                //encontramos un gris que en la solucion es rojo
                if(board[x][y].currentState == Square.SquareColor.Grey  &&
                        board[x][y].solutionState == Square.SquareColor.Red) {
                    return board[x][y];
                }
                else if(board[x][y].solutionState == Square.SquareColor.Red ) {//encontramos un rojo
                    break;
                }
                x += d.getRow();
                y += d.getCol();
            }
        }
        return board[0][0];//no se ha encontrado ninguno (el square pasado por parametro no era el correcto)
    }


    //Devuelve una pista
    public String giveHint(){
        Random rnd = new Random();
        //para poder generar pistas en diferentes casillas cada vez que
        // se llame al metodo (cuando el jugador quiere una pista)
        int pos = rnd.nextInt(locked.size());
        int i= pos;
        do{
            Square s = locked.get(i);
            if(hint1(s,false)){
                //_hintedSquare = s;
                if(_drawBlackSquare != null && _drawBlackSquare != s)
                    _drawBlackSquare.drawBlack=false;
                _drawBlackSquare =  s;
                _drawBlackSquare.drawBlack = true;
                countRow(s.posX,true);
                countCol(s.posY,true);
                return Hint.CanClose.getMsg();
            }
            else if(hint2(s,false)){
                //_hintedSquare = s;
                if(_drawBlackSquare != null && _drawBlackSquare != s)
                    _drawBlackSquare.drawBlack=false;
                _drawBlackSquare =  s;
                _drawBlackSquare.drawBlack = true;
                countRow(s.posX,true);
                countCol(s.posY,true);
                return Hint.WouldSeeTooMuch.getMsg();

            }
            else if(hint3(s,false)){
                System.out.println("Pista 3 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                //_hintedSquare = s;
                if(_drawBlackSquare != null && _drawBlackSquare != s)
                    _drawBlackSquare.drawBlack=false;
                _drawBlackSquare =  s;
                _drawBlackSquare.drawBlack = true;
                countRow(s.posX,true);
                countCol(s.posY,true);
                return Hint.WouldSeeTooLittle.getMsg();

            }
            else if(hint4(s)){
                System.out.println("Pista 4 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                //_hintedSquare = s;
                if(_drawBlackSquare != null && _drawBlackSquare != s)
                    _drawBlackSquare.drawBlack=false;
                _drawBlackSquare =  s;
                _drawBlackSquare.drawBlack = true;
                countRow(s.posX,true);
                countCol(s.posY,true);
                return Hint.SeesTooMuch.getMsg();

            }
            else if(hint5(s)){
                System.out.println("Pista 5 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                //_hintedSquare = s;
                if(_drawBlackSquare != null && _drawBlackSquare != s)
                    _drawBlackSquare.drawBlack=false;
                _drawBlackSquare =  s;
                _drawBlackSquare.drawBlack = true;
                countRow(s.posX,true);
                countCol(s.posY,true);
                return Hint.SeesToLittle.getMsg();

            }
            i++;
            i %= locked.size();
        }while(i != pos);

        //Si no ha encontrado ninguna pista, se busca poner rojos obligatorios
        for( i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j) {
                if(!board[i][j].lock)
                    if(hint6_7(board[i][j], false)) {
                        //_hintedSquare = (board[i][j]);
                        if(_drawBlackSquare != null && _drawBlackSquare != board[i][j])
                            _drawBlackSquare.drawBlack=false;
                        _drawBlackSquare =  board[i][j];
                        _drawBlackSquare.drawBlack = true;
                        System.out.println("Pista 6/7 aceptada en " + (board[i][j].posX - 1)
                                + " " + (board[i][j].posY - 1));

                        if(board[i][j].currentState == Square.SquareColor.Blue)
                            return Hint.MustBeRedBlue.getMsg();
                        return Hint.MustBeRedGrey.getMsg();
                    }

            }
        }
        return "";
    }

    // Aplica pistas. Se usa para terminar de generar un nivel soluble
    public boolean doHint(){

        for(Square s : locked){
            if(hint1(s,true)){
                //System.out.println("Pista 1 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint2(s,true)){
                //System.out.println("Pista 2 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint3(s,true)){
                //System.out.println("Pista 3 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint4(s)){
                //System.out.println("Pista 4 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint5(s)){
                //System.out.println("Pista 5 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
        }

        //si alguno de los locked no estan completos, no se puede completar el nivel
        for(Square s : locked) {
            if (s.total != s.playerColumn + s.playerRow) {
                return false;
                //break;
            }
        }

        for(int i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j) {
                if(!board[i][j].lock)
                    if(hint6_7(board[i][j], true)) {
                        //System.out.println("Pista 6/7 aceptada en " + (board[i][j].posX - 1)
                        //+ " " + (board[i][j].posY - 1));
                        countRow(board[i][j].posX,true);
                        countCol(board[i][j].posY,true);
                        return true;
                    }

            }
        }
        return false;
    }

    //devuelve true si ve suficiente para cerrarlo y no esta cerrado.
    private boolean hint1(Square square, boolean modify){
        boolean check = square.total == square.playerColumn + square.playerRow;
        if(check) {
            for (Dirs d : Dirs.values()) {//buscar en todas las direcciones
                int x = square.posX;
                int y = square.posY;

                while (board[x][y].currentState == Square.SquareColor.Blue) {
                    x += d.getRow();
                    y += d.getCol();
                }//ha encontrado una casilla que no es azul

                //si es gris se cumple la pista
                if(board[x][y].currentState == Square.SquareColor.Grey){
                    if(modify)
                        board[x][y].currentState = Square.SquareColor.Red;
                    return true;
                }
            }
        }
        return  false;
    }

    //devuelve true si veria demasiados. Es decir, tiene que poner un rojo en cierta direccion
    private boolean hint2(Square square, boolean modify){
        for (Dirs d : Dirs.values()) {//buscar en todas las direcciones
            int x = square.posX;
            int y = square.posY;
            while (board[x][y].currentState == Square.SquareColor.Blue) {
                x += d.getRow();
                y += d.getCol();
            }//ha encontrado un gris o un rojo

            if(board[x][y].currentState == Square.SquareColor.Grey &&   // Hay un gris seguido de un azul
                    board[x + d.getRow()][y + d.getCol()].currentState == Square.SquareColor.Blue){
                //x += d.getRow();y += d.getCol();
                if(d == Dirs.UP || d == Dirs.DOWN){ // Columna
                    if((board[x+d.getRow()][y+d.getCol()].playerColumn + (square.playerRow + square.playerColumn + 1))
                            > (square.total)) {
                        if(modify)//modificar
                            board[x][y].currentState = Square.SquareColor.Red;
                        return true;    // Más grande que la solución
                    }
                }
                else{                              // Fila
                    if((board[x+d.getRow()][y+d.getCol()].playerRow + (square.playerRow + square.playerColumn + 1))
                            > (square.total)){
                        if(modify) {
                            board[x][y].currentState = Square.SquareColor.Red;//modificar

                        }
                        return true;    // Más grande que la solución
                    }
                }
            }
        }
        return false;
    }

    private boolean hint3(Square square, boolean modify){
        //guardamos los que puede ver y los que ya ve por cada direccion
        //vamos a guardar en las 4 direcciones los que pueden ser azules,
        //los que actualmente son azules
        //y la direccion a la que llega ahi (x, y) 1,0 = derecha
        Integer[][] posAdyCount = new Integer[4][4];
        int i=0;
        int posCount=0;
        int blueCount=0;
        for(Dirs d: Dirs.values()){//recorrer en todas las direcciones
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            boolean countBlue = true;//cuentas los azules adyacentes
            while(board[x][y].currentState != Square.SquareColor.Red){
                if(board[x][y].currentState == Square.SquareColor.Grey )//guardamos los azules en esa dir
                    countBlue = false;
                else if(countBlue)
                    blueCount++;
                posCount++;//guardamos todos los posibles azules
                x += d.getRow();
                y += d.getCol();
            }

            posAdyCount[i][0]=posCount;
            posAdyCount[i][1]=blueCount;
            posAdyCount[i][2]=d.getRow();
            posAdyCount[i][3]=d.getCol();
            posCount=blueCount=0;
            i++;
        }//fin buscar en 4 direcciones los posibles azules

        //ordenamos de menor a mayor los posibles azules menos los que ya son azules
        for( i = 0; i < posAdyCount[0].length -1; ++i){
            for(int j = 0; j < posAdyCount[0].length -1; ++j){
                int tmp = posAdyCount[j][0] - posAdyCount[j][1];
                int aux = posAdyCount[j+1][0] - posAdyCount[j+1][1];
                if(tmp > aux){
                    tmp = posAdyCount[j][0];
                    aux = posAdyCount[j][1];
                    int aux2 = posAdyCount[j][2];
                    int aux3 = posAdyCount[j][3];
                    posAdyCount[j][0] = posAdyCount[j+1][0];
                    posAdyCount[j][1] = posAdyCount[j+1][1];
                    posAdyCount[j][2] = posAdyCount[j+1][2];
                    posAdyCount[j][3] = posAdyCount[j+1][3];

                    posAdyCount[j+1][0] = tmp;
                    posAdyCount[j+1][1] = aux;
                    posAdyCount[j+1][2] = aux2;
                    posAdyCount[j+1][3] = aux3;
                }
            }
        }//ya esta ordenado de menor a mayor

        //sumamos los posibles en todas las direcciones menos
        // la que estamos comprobando (tiene mas posibles)
        int count=0;
        for(i=0;i<3;++i){
            count += posAdyCount[i][0];
        }

        if(modify && count < square.total - posAdyCount[3][1]){
            int x= square.posX + posAdyCount[3][2];
            int y= square.posY + posAdyCount[3][3];
            while(board[x][y].currentState == Square.SquareColor.Blue) {
                x += posAdyCount[3][2];
                y += posAdyCount[3][3];
            }
            if(board[x][y].currentState != Square.SquareColor.Red)
                board[x][y].currentState = Square.SquareColor.Blue;
            //actualizar valores de los que ve en fila y columna

        }

        return count < square.total - posAdyCount[3][1];
    }

    //Ve más de los que puede ver
    private boolean hint4(Square square){
        return square.total < square.playerColumn + square.playerRow;
    }

    //Tiene que ver más y ya está cerrado
    private boolean hint5(Square square){
        //si ves menos de los que deberias y al final de tus adyacentes azules hay un rojo
        if(square.total > square.playerColumn + square.playerRow){
            for(Dirs d: Dirs.values()){//buscar en todas direcciones
                int x = square.posX + d.getRow();
                int y = square.posY + d.getCol();
                while(board[x][y].currentState != Square.SquareColor.Red){
                    //si encuentra un gris significa que no esta rodeado por rojos
                    if(board[x][y].currentState == Square.SquareColor.Grey)
                        return false;
                    x += d.getRow();
                    y += d.getCol();
                }
            }
            //si no ha encontrado un gris al mirar en todas las direcciones esta cerrado por rojos
            return true;
        }
        return false;
    }

    //devuelve si el square esta rodeado por rojos
    //le tienen que llegar square azules o grises
    private boolean hint6_7(Square square, boolean modify){
        if(square.currentState == Square.SquareColor.Red ) return false;
        for(Dirs d: Dirs.values()){//buscar en todas direcciones que no haya un azul
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            while(board[x][y].currentState == Square.SquareColor.Grey ||
                    (board[x][y].currentState == Square.SquareColor.Blue && !board[x][y].lock)){
                x += d.getRow();
                y += d.getCol();
            }
            //Si el que encuentra es azul no esta rodeado por rojos
            if(board[x][y].currentState == Square.SquareColor.Blue && board[x][y].lock)
                return false;
        }//deja de buscar. Rodeado por rojos

        if(modify )
            square.currentState = Square.SquareColor.Red;
        return true;
    }

    /**
     * Metodo para mostrar por consola el tablero solucion
     */
    public void printSolution(){
        System.out.println();
        for(int i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j){
                System.out.print((board[i][j].total) + " " );
            }
            System.out.println();
        }
    }

    /**
     * Metodo para mostrar por consola el tablero
     */
    public void print(){
        System.out.println();

        //muestra el estado del tablero
        for(int i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j){
                if(board[i][j].lock && board[i][j].currentState == Square.SquareColor.Blue)
                    System.out.print(board[i][j].total+" ");
                else if(board[i][j].currentState == Square.SquareColor.Blue)
                    System.out.print("A");
                else if(board[i][j].currentState == Square.SquareColor.Grey)
                    System.out.print("0");
                else if(board[i][j].currentState == Square.SquareColor.Red)
                    System.out.print("R");
            }
            System.out.println();
        }
    }


    /**
     *  Cuenta el numero de azules en una fila
     * @param i la fila a calcular
     * @param player si es true es sobre el tablero que ve el jugador. false sobre la solucion
     */
    private void countRow(int i, boolean player){
        int j=1;
        if(!player){
            while(j != board[0].length){
                if(board[i][j].row == 0 && board[i][j].solutionState == Square.SquareColor.Blue)
                    countRowRec(i, j,0, player);
                j++;
            }
        }
        else{
            while(j != board[0].length){
                if( board[i][j].currentState == Square.SquareColor.Blue
                        && board[i][j-1].currentState != Square.SquareColor.Blue)
                    countRowRec(i, j,0, player);
                j++;
            }
        }
    }


    /**
     * Calcula de manera recursiva el numero de azules en esa fila
     * la condicion de parada es encontrar una casilla roja o el final
     * @param i la fila
     * @param j la columna
     * @param cont el numero que ha contado de adyacentes en esa fila
     * @param player true se calcula en funcion al tablero que ve el jugador. false con el tablero solucion
     * @return devuelve el numero de adyacentes encontrados (cont)
     */
    private int countRowRec(int i, int j, int cont, boolean player){
        if(!player) {
            if (j == board[0].length - 1) {//si llegamos al final
                board[i][j].row = cont;
                return cont;
            }
            if (board[i][j + 1].solutionState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].row = countRowRec(i, j + 1, ++cont, player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].row = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].row;
        }
        else{
            if (j == board[0].length - 1) {//si llegamos al final
                board[i][j].playerRow = cont;
                return cont;
            }
            if (board[i][j + 1].currentState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].playerRow = countRowRec(i, j + 1, ++cont, player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].playerRow = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].playerRow;
        }
    }


    /**
     *  Cuenta el numero de azules en una columna
     * @param j la columna a calcular
     * @param player si es true es sobre el tablero que ve el jugador. false sobre la solucion
     */
    private void countCol(int j,boolean player){
        int i=1;
        if(!player) {
            while (i != board[0].length) {
                if (board[i][j].column == 0 && board[i][j].solutionState == Square.SquareColor.Blue)
                    countColRec(i, j, 0, player);
                i++;
            }
        }
        else{
            while (i != board[0].length) {
                if (board[i][j].currentState == Square.SquareColor.Blue && board[i-1][j].currentState != Square.SquareColor.Blue)
                    countColRec(i, j, 0, player);
                i++;
            }
        }
    }


    /**
     * Calcula de manera recursiva el numero de azules en esa columna
     * la condicion de parada es encontrar una casilla roja o el final
     * @param i la fila
     * @param j la columna
     * @param cont el numero que ha contado de adyacentes en esa columna
     * @param player true se calcula en funcion al tablero que ve el jugador. false con el tablero solucion
     * @return devuelve el numero de adyacentes encontrados (cont)
     */
    private int countColRec(int i, int j, int cont,boolean player) {
        if (!player) {
            if (i == board[0].length - 1) {//si llegamos al final
                board[i][j].column = cont;
                return cont;
            }
            if (board[i + 1][j].solutionState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].column = countColRec(i + 1, j, ++cont, player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].column = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].column;
        } else {
            if (i == board[0].length - 1) {//si llegamos al final
                board[i][j].playerColumn = cont;
                return cont;
            }
            if (board[i + 1][j].currentState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].playerColumn = countColRec(i + 1, j, ++cont, player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].playerColumn = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].playerColumn;
        }
    }

    /**
     * //revela a los que no son visibles por otro azul en esa fila
     * @param i es la fila a calcular
     */
    private void markShowInRow(int i){
        int j=1;
        while(board[0].length -1 != j){
            if(!board[i][j].lock  && board[i][j].solutionState == Square.SquareColor.Blue) {
                if(board[i][j-1].showInRow  || (board[i][j-1].lock &&
                        board[i][j-1].currentState == Square.SquareColor.Blue))//el anterior es azul
                    board[i][j].showInRow = true;
                    //el de la izquierda es rojo y el de la derecha no
                else if(board[i][j+1].solutionState != Square.SquareColor.Red)
                {
                    board[i][j].lock = true;
                    locked.add(board[i][j]);
                    board[i][j].showInRow = true;
                    board[i][j].currentState = board[i][j].solutionState;
                    //_numGreys--;
                }
            }
            j++;
        }
    }


    /**
     *  revela a los que no son visibles por otro azul en esa columna
     * @param j es la columna a calcular
     */
    private void markShowInCol(int j){
        int i=1;
        while( board[0].length -1 != i){
            if(!board[i][j].lock && board[i][j].solutionState == Square.SquareColor.Blue) {
                if(board[i-1][j].showInColumn  || (board[i-1][j].lock &&
                        board[i-1][j].currentState == Square.SquareColor.Blue) )//el anterior es azul
                    board[i][j].showInColumn = true;
                    //el de arriba es rojo y el de abajo no
                else if(board[i+1][j].solutionState != Square.SquareColor.Red)
                {
                    board[i][j].lock = true;
                    locked.add(board[i][j]);
                    board[i][j].showInColumn = true;
                    board[i][j].currentState = board[i][j].solutionState;
                    //_numGreys--;
                }
            }
            i++;
        }
    }

    private void reveal(){
        int size = board[0].length;
        Random r = new Random();

        for(int i = 1; i < size -1; ++i){
            for(int j = 1; j < size -1; ++j){
                // comprobar que no se ha generado aislado
                if((board[i][j].total) == 0 && board[i][j].solutionState == Square.SquareColor.Blue){
                    board[i][j].solutionState = Square.SquareColor.Red;
                }
                if(board[i][j].solutionState == Square.SquareColor.Red){
                    if(r.nextFloat() >=0.6f) {
                        board[i][j].lock = true;
                        board[i][j].currentState = Square.SquareColor.Red;
                        //_numGreys--;
                    }
                }
            }
        }

        for(int i = 1; i < board[0].length -1; ++i){
            markShowInRow(i);   //revela los necesarios en filas
            markShowInCol(i);   //revela los nocesarios en columnas
        }
    }

    private void activateCell(int indexX, int indexY, boolean leftMouse, boolean recordMove){
       // if(_hintedSquare != null) _hintedSquare = null;
        Square activated = board[indexY+1][indexX+1];
        if(activated.currentState == Square.SquareColor.Grey)//vamos a quitar un gris
            _numGreys--;

        if(!activated.lock){
            activated.prevState = activated.currentState;
            if(leftMouse) activated.currentState = Square.SquareColor.values()[//has pulsado con click izquierdo(en android siempre)
                    (activated.currentState.ordinal() + 1) % Square.SquareColor.values().length];
            else{//has pulsado con click derecho
                int newIndex = activated.currentState.ordinal() - 1;
                if(newIndex < 0) newIndex = Square.SquareColor.values().length - 1;
                activated.currentState = Square.SquareColor.values()[newIndex];
            }
            if(activated.currentState == Square.SquareColor.Grey)//hemos añadido un gris
                _numGreys++;
            countRow(activated.posX,true);
            countCol(activated.posY,true);
            // Se añade un movimienot CONTRARIO (!leftMouse) al stack
            if(recordMove)
                _oppositeMoves.add(new TouchEvent(TouchType.Press, indexX, indexY, 0, !leftMouse));
            activated.beginFading();
        }else{
            _showLock = !_showLock;
            //vibrar
            activated.beginVibration();
        }
    }

    private void undoMove(){
        if(_drawBlackSquare != null )
            _drawBlackSquare.drawBlack=false;
        /*if (_hintedSquare != null){
            _hintedSquare = null;
            _hintedSquareCircle = null;
            if(_hintUndoText != "") {
                _hintUndoText = "";
                _sizeTextFadeFactor = 1;
                _hintUndoTextFadeFactor = -1;
            }

        }
        else*/ if(_oppositeMoves.size() > 0) {
            TouchEvent e = _oppositeMoves.pop();
            activateCell((int) e.getX(), (int) e.getY(), e.isRightMouse(), false);
            _undoOnce = true;
            Square square = board[(int) e.getY()+1][(int) e.getX()+1];
            switch (square.currentState){
                case Red:
                    _hintUndoText = "Casilla vuelta roja";
                    break;
                case Blue:
                    _hintUndoText = "Casilla vuelta azul";
                    break;
                case Grey:
                    _hintUndoText = "Casilla vuelta gris";
                    break;
            }

            _drawBlackSquare =  square;
            _drawBlackSquare.drawBlack = true;
            _sizeTextFadeFactor = -1;
            _hintUndoTextFadeFactor = 1;

        }
        else if(_hintUndoText == ""){
            if(!_undoOnce) {
                _hintUndoText = "Boton de deshacer";
                _sizeTextFadeFactor = -1;
                _hintUndoTextFadeFactor = 1;
            }
            else {
                if(_hintUndoText == "") {
                    if(_drawBlackSquare != null ) {
                        _drawBlackSquare.drawBlack = false;
                        _drawBlackSquare=null;
                    }

                    _hintUndoText = "Nada que deshacer";
                    _sizeTextFadeFactor = -1;
                    _hintUndoTextFadeFactor = 1;
                }

            }
        }
        else{
            _hintUndoText = "";
            if(_drawBlackSquare != null ) {
                _drawBlackSquare.drawBlack = false;
                _drawBlackSquare=null;
            }
            _sizeTextFadeFactor = 1;
            _hintUndoTextFadeFactor = -1;
        }
    }
}
