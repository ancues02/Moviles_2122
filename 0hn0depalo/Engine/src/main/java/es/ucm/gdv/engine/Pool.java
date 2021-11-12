package es.ucm.gdv.engine;

import java.util.LinkedList;

public class Pool<T> {
    T[] _pool;
    int _size;
    LinkedList<Integer> _free;

    public Pool(int maxSize, Factory<T> factory){
        this._size = maxSize;
        for(int i = 0; i < _size; i++){
            _pool[i] = factory.newInstance();
            _free.push(i);
        }
    }

    public T obtain(){
        if(!_free.isEmpty()){
            System.err.println("Could not instantiate object, pool is full");
            return null;
        }
        else{
            return _pool[_free.pop()];
        }
    }

    public void release(T obj){
        int i = 0;
        while(i < _pool.length){
            if(_pool[i]==obj){
                _free.add(i);
            }
        }
    }
}
