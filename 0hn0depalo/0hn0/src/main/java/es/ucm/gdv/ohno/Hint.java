package es.ucm.gdv.ohno;

public enum Hint {
    // Pistas sobre puestas
    CanClose ("Esta casilla ya ve lo suficiente"),
    WouldSeeTooMuch (" No puede poner en una direccion"),
    WouldSeeTooLittle (" Una direccion asegurada"),
    // Errores sobre puestas
    SeesTooMuch (" Ve demasiado"),
    SeesToLittle (" No ve suficiente"),
    // Pistas sobre cerradas
    MustBeRedGrey("Tiene que ser rojo"),
    MustBeRedBlue("Tiene que ser rojo");

    private final String msg;

    Hint(String s){
        this.msg = s;
    }
}
