import os
from os.path import join, dirname
from openai import OpenAI
from dotenv import load_dotenv

API_KEY = ""

def DoAPIRequest(difficulty, theme):

	#load_dotenv()

	OpenAI.api_key = API_KEY #os.getenv("API_KEY")

	context_prompt = "Pretend you are hosting a gameshow where the player tries to answer the questions. Write a question that is based on the given theme and difficulty, and 4 short answers. The difficulty goes from 0 to 10, where 0 is something a small child can answer, and 10 is something only experts can answer. The first answer is always the correct one. Give the final question and answers as valid JSON."

	prompt_json1 = open('./prompt_part1.json', 'r')
	prompt_json2 = open('prompt_part2.json', 'r')

	messages = [ {"role": "system", "content": context_prompt + " question1:" + prompt_json1 + " example question2(0, Food): " + prompt_json2} ]

	#while True: 
		#message = input("User : ") 
		#if message: 
	messages.append( 
		{"role": "user", "content": "question3(0, Math):"}, 
	) 
	chat = OpenAI.ChatCompletion.create( 
		model="gpt-3.5-turbo", messages=messages 
	)
	  
	reply = chat.choices[0].message.content 
	return reply
	#print(f"ChatGPT: {reply}") 
	#messages.append({"role": "assistant", "content": reply})
