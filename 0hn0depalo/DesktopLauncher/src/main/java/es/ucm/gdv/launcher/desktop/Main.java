package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.Scene;
import es.ucm.gdv.engine.desktop.DesktopEngine;
import es.ucm.gdv.ohno.OhnO_Menu;

public class Main {
    public static void main(String[] args) {
        DesktopEngine e = new DesktopEngine("0hn0 del palo",1920, 1080, 600, 900);
        Scene a = new OhnO_Menu();
        e.setScene(a);
        //No tiene sentido que este en la interfaz Engine porque en movil no se va a cambiar el tamaño de la ventana.
        e.setSize(1080 / 3,1584 / 3);//esto tiene que estar implementado en PCEngine, no en la interfaz Engine. La razon es que la logica no se va a usar.
        e.run();
    }
}