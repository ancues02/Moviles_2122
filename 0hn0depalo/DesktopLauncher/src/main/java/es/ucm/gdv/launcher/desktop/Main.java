package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.ohno.*;

public class Main {
    public static void main(String[] args) {
        Logic loc = new Logic(4);
        loc.print();
        loc.giveHint();
    }
}