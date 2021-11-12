package es.ucm.gdv.ohno;

public abstract class FadeObject {

    protected float animTime;
    protected float alpha;

    protected FadeObject(float iniAlpha, float animTime){
        this.alpha = iniAlpha;
        this.animTime = animTime;
    }

    public void fadeOut(float deltaTime){
        alpha -= deltaTime * (255.0f / animTime);
        alpha = clamp(alpha, 0.0f, 255.0f);
    }

    protected void fadeIn(float deltaTime){
        alpha += deltaTime * (255.0f / animTime);
        alpha = clamp(alpha, 0.0f, 255.0f);
    }

    public abstract void update(float deltaTime);

    protected float clamp(float value, float min, float max){
        float ret = value;
        ret = Math.max(min, ret);
        ret = Math.min(ret, max);
        return ret;
    }
}
