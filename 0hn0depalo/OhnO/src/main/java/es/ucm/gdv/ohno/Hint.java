package es.ucm.gdv.ohno;

public enum Hint {
    // Pistas sobre puestas
    CanClose ("Se puede cerrar"),
    WouldSeeTooMuch (" Prohibida una direccion"),
    WouldSeeTooLittle ("Direccion asegurada"),
    // Errores sobre puestas
    SeesTooMuch ("Ve demasiado"),
    SeesToLittle ("No ve suficiente"),
    // Pistas sobre cerradas
    MustBeRedGrey("Deberia ser facil..."),
    MustBeRedBlue("No lo ve ningun azul");

    private final String msg;
    public String getMsg(){return this.msg;}
    Hint(String s){
        this.msg = s;
    }
}
