package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.desktop.DesktopEngine;
import es.ucm.gdv.ohno.*;
public class Main {
    public static void main(String[] args) {
        DesktopEngine e = new DesktopEngine(1920, 1080, 600, 900);
        Application a = new OhnO(9);
        e.setApplication(a);//esto tiene que estar implementado en PC/AEngine, no en la interfaz Engine. La razon es que la logica no se va a usar.
        //No tiene sentido que este en la interfaz Engine porque en movil no se va a cambiar el tamaño de la ventana(no hay opcion xd).
        e.setSize(1080 / 3,1584 / 3);//esto tiene que estar implementado en PCEngine, no en la interfaz Engine. La razon es que la logica no se va a usar.
        e.run();
    }
}