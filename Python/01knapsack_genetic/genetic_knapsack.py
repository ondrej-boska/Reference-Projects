import numpy as np
import random
import copy
import matplotlib.pyplot as plt
from deap import tools

input_file = open("input_data_1000.txt", 'r')
line = input_file.readline()
items_count, max_weight = int(line.split()[0]), int(line.split()[1])
items = [(int(line.split()[0]), int(line.split()[1])) for line in input_file]
input_file.close()

def random_population(population_size, individual_size):
    return [np.array(np.random.permutation(individual_size)) for _ in range(population_size)]

def fitness(individual):
    curr_cost = 0
    curr_weight = 0
    idx = 0
    while(idx < items_count and (curr_weight + items[individual[idx]][1]) <= max_weight):
        curr_cost += items[individual[idx]][0]
        curr_weight += items[individual[idx]][1]
        idx += 1
    return curr_cost

# roulette selection
def selection(population,fitness_value):
    return copy.deepcopy(random.choices(population, weights=fitness_value, k=len(population)))

# PMX from Deap package, returnes crossovered population
def crossover(population,cross_prob):
    new_population = []
    for i in range(0,len(population)//2):
        indiv1 = copy.deepcopy(population[2*i])
        indiv2 = copy.deepcopy(population[2*i+1])
        if random.random()<cross_prob:
            tools.cxPartialyMatched(indiv1, indiv2)
        new_population.append(indiv1)
        new_population.append(indiv2)
    return new_population

# switches two numbers in a permutation, returns mutated population
def mutation(population, probability):
    new_population = [copy.deepcopy(individual) for individual in population]
    for individual in new_population:
        if random.random() < probability:
            idx1 = random.randint(0, len(individual) - 1)
            idx2 = random.randint(0, len(individual) - 1)
            individual[idx1], individual[idx2] = individual[idx2], individual[idx1]
    return new_population    

def elitism(old_population, new_population, fitness_value, count):
    fitness_value = copy.deepcopy(fitness_value)
    fitness_value_for_argmin = copy.deepcopy(fitness_value)
    indicies = []
    for i in range(count):
        worst_fit = np.argmin(fitness_value_for_argmin)
        del fitness_value_for_argmin[worst_fit]
        indicies.append(worst_fit)
    for i in range(count):
        best_fit = np.argmax(fitness_value)
        new_population[indicies[i]] = old_population[best_fit]
        fitness_value[indicies[i]] = 0


def evolution(population_size, individual_size, max_generations):
    max_fitness = []
    population = random_population(population_size,individual_size)
    mut_prob = 1
    elitism_count = int(population_size * 0.1)
    
    for i in range(0,max_generations):
        fitness_value = list(map(fitness, population))
        max_fitness.append(max(fitness_value))
        parents = selection(population,fitness_value)
        children = crossover(parents, 0.6)
        mut_children = mutation(children, mut_prob)
        elitism(population, mut_children, fitness_value, elitism_count)
        population = mut_children
        if(i%100 == 0):
            print(i)
            print(max(max_fitness))
            #mut_prob *= 0.95
        
    fitness_value = list(map(fitness, population))
    max_fitness.append(max(fitness_value))
    best_individual = population[np.argmax(fitness_value)]
    
    return best_individual, population, max_fitness

ultra_max = 0
ultra_max_fitness = []
for i in range(1):
    best, population, max_fitness = evolution(population_size=300,individual_size=items_count,max_generations=8000)
    print(max(max_fitness))
    if(max(max_fitness)>ultra_max):
        ultra_max = max(max_fitness)
        ultra_max_fitness = max_fitness
print(ultra_max)    

print('best individual: ', best)

plt.plot(ultra_max_fitness)
plt.ylabel('Fitness')
plt.xlabel('Generace')
plt.show() 