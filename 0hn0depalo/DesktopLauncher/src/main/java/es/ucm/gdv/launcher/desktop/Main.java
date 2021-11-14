package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.Scene;
import es.ucm.gdv.engine.desktop.DesktopEngine;
import es.ucm.gdv.ohno.OhnO_Menu;

public class Main {
    public static void main(String[] args) {
        DesktopEngine e = new DesktopEngine("0hn0 del palo",1920, 1080, 600, 900);
        Scene a = new OhnO_Menu();
        e.setScene(a);
        e.setSize(1080 / 3,1584 / 3); // Método no necesario anda mas que para que el profe vea que lo tenemos.
        e.run();
    }
}