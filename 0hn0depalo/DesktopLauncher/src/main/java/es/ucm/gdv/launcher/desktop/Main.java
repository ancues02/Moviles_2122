package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.engine.desktop.DesktopGraphics;
import es.ucm.gdv.engine.desktop.DesktopImage;
import es.ucm.gdv.ohno.*;

public class Main {
    public static void main(String[] args) {
        DesktopGraphics degra = new DesktopGraphics(400, 1080);
        Logic loc = new Logic(4);
        loc.print();
        loc.giveHint();
    }
}