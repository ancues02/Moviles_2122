package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.desktop.DesktopGraphics;
import es.ucm.gdv.engine.desktop.DesktopImage;
import es.ucm.gdv.ohno.*;
import  java.lang.Thread;
public class Main {
    public static void main(String[] args) {
        //DesktopGraphics degra = new DesktopGraphics(400, 1080);
        Logic loc = new Logic();
        loc.init(4);
        loc.print();
        while(!loc.check() && loc.doHint() ) {
            loc.print();
            //if(!loc.doHint())
              //  loc.init(4);
            /*try {

                Thread.sleep(1000);

            } catch (InterruptedException e) {


            }
            System.out.flush();*/

        }
    }
}