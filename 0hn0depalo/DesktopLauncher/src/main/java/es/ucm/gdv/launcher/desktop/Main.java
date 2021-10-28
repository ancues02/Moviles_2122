package es.ucm.gdv.launcher.desktop;
import es.ucm.gdv.ohno.*;
import  java.lang.Thread;
public class Main {
    public static void main(String[] args) {
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