package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.desktop.DesktopGraphics;
import es.ucm.gdv.engine.desktop.DesktopImage;
import es.ucm.gdv.ohno.*;
import  java.lang.Thread;
public class Main {
    public static void main(String[] args) {
        DesktopGraphics degra = new DesktopGraphics(1920, 1080, 1080, 1920);
        //degra.clear(255, 255, 255, 255);
        degra.setColor(255, 0, 0, 255);
        degra.fillCircle(300, 500, 100);
        degra.present();

        /*Logic loc = new Logic();
        loc.init(9);
        loc.printSolution();
        loc.print();
        int i = 0;
        int resetCount= 0;
        while(!loc.check() &&  i < 1000) {
            if(!loc.doHint()){
                System.out.println("REINICIAR");
                loc.reStart(false);
                loc.printSolution();
                resetCount++;
            }
            loc.print();
            ++i;

        }
        //loc.printSolution();

        System.out.println("He necesitado " + i +" iteraciones\nMe he reiniciado "+ resetCount +" veces");*/
    }
}