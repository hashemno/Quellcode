
class Persona:

    def __init__(self,id,name,age,genre,introvertion,nervousness) :
        self.id=id
        self.name=name
        self.age=age
        self.genre=genre
        self.introvertion=introvertion
        self.nervousness=nervousness
        self.chat_history={}

    def get_attributes(self):
        return f"Name={self.name},Age={self.age},Genre={self.genre},Introvertion/10={self.introvertion},Nervousness/10={self.nervousness}"
    
    def to_dict(self):
        return {"id":self.id,"name": self.name, "age": self.age, "genre": self.genre,"introvertion":self.introvertion,"nervousness":self.nervousness}
    
    def update_chat(self,messenger,message):
        self.chat_history+= messenger+message
    """
    update Chat history with the new string from either AI or Player.
    Parameters:
    messenger (string): either AI's name or Player's name 
    message (string): the new message
    """