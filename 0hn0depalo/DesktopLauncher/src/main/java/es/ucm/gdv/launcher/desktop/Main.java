package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.desktop.DesktopGraphics;
import es.ucm.gdv.engine.desktop.DesktopImage;
import es.ucm.gdv.ohno.*;
import  java.lang.Thread;
public class Main {
    public static void main(String[] args) {
        DesktopGraphics degra = new DesktopGraphics(1920, 1080, 720, 1280);
        //degra.clear(255, 0, 0, 255);
        degra.fillCircle(400.0f, 400.0f, 50);
        Logic loc = new Logic(4);
        //loc.print();
        while(loc.doHint()) {
            loc.print();
            //if(!loc.doHint())
                //loc.init(4);
            /*try {

                Thread.sleep(1000);

            } catch (InterruptedException e) {


            }
            System.out.flush();*/

        }
    }
}