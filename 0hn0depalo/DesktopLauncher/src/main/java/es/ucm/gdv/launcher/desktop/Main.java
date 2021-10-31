package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.desktop.DesktopEngine;
import es.ucm.gdv.engine.desktop.DesktopGraphics;
import es.ucm.gdv.engine.desktop.DesktopImage;
import es.ucm.gdv.ohno.*;
public class Main {
    public static void main(String[] args) {
        //DesktopGraphics degra = new DesktopGraphics(1920, 1080, 600, 900);
        DesktopEngine e = new DesktopEngine();

            //Engine e = new es.ucm.gdv.engine.desktop.Engine();//es lo mismo que PCEngine

        Application a =new OhnO(4);


        e.setApplication(a);//esto tiene que estar implementado en PC/AEngine, no en la interfaz Engine. La razon es que la logica no se va a usar.

        //No tiene sentido que este en la interfaz Engine porque en movil no se va a cambiar el tamaño de la ventana(no hay opcion xd).
        //e.setSize(1024,720);//esto tiene que estar implementado en PCEngine, no en la interfaz Engine. La razon es que la logica no se va a usar.

        e.run();


        //while(true) {
            //degra.clear(255, 0, 0, 255);
            //degra.setColor(255, 0, 0, 255);
            //degra.fillCircle(300, 500, 100);

       // }

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